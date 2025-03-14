using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DodgeGameLobby : MonoBehaviour
{
    private const string RELAY_KEY_JOIN_CODE = "relayJoinCode";
    public static DodgeGameLobby Instance{get;private set;}
    private Lobby joinedLobby;
    public event EventHandler OnCreateLobby;
    public event EventHandler OnCreateLobbyFailed;
    public event EventHandler OnLobbyJoin;
    public event EventHandler OnLobbyJoinFailed;

    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs: EventArgs{
        public List<Lobby> lobbyList;
    }

    private float hearBeatTimer;
    private float lobbyRefreshTimer;
    private void Awake(){
        Instance = this;
        DontDestroyOnLoad(this);

        InitializeUnityAuthentication();
    }
    private void Start(){
        if(joinedLobby!=null){
            LeavePlayerFromLobby();
        }
    }

    private void Update(){
        HandleHeartBeat();
        HandleRefreshLobbyTimer();
    }
    private void HandleRefreshLobbyTimer(){
        if(joinedLobby == null && AuthenticationService.Instance.IsSignedIn && SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString()){
            lobbyRefreshTimer -= Time.deltaTime;
            if(lobbyRefreshTimer < 0){
                GetLobbiesList();
                lobbyRefreshTimer = 3f;
            }
        }
    }

    private void HandleHeartBeat(){
        if(IsLobbyHost()){
            hearBeatTimer -= Time.deltaTime;
            if(hearBeatTimer < 0){
                Debug.Log("hearthu beatuu");
                hearBeatTimer = 15f;
                LobbyService.Instance.SendHeartbeatPingAsync(joinedLobby.Id);
            }
        }
    }

    private bool IsLobbyHost(){
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }
    private async void InitializeUnityAuthentication(){
        if(UnityServices.State != ServicesInitializationState.Initialized){
            InitializationOptions initializationOptions = new InitializationOptions();
            initializationOptions.SetProfile(UnityEngine.Random.Range(0 ,1000).ToString());
            await UnityServices.InitializeAsync();

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    private async Task<Allocation> AllocateRelay(){
        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(DodgeGameMultiplayer.MAX_PLAYER_COUNT-1);
            return allocation;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }
    private async Task<string> GetRelayJoinCode(Allocation allocation){
        try{
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            return joinCode;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode){
        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            return joinAllocation;
        }catch(RelayServiceException e){
            Debug.Log(e);
            return default;
        }
    }
    public async void CreateLobby(string lobbyName ,bool isPrivate){
        OnCreateLobby?.Invoke(this , System.EventArgs.Empty);
        try{
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName , DodgeGameMultiplayer.MAX_PLAYER_COUNT ,new CreateLobbyOptions{
                IsPrivate = isPrivate
            });
            Allocation allocation = await AllocateRelay();
            
            string joinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(joinedLobby.Id , new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>{
                    {RELAY_KEY_JOIN_CODE ,new DataObject(DataObject.VisibilityOptions.Member , joinCode)}
                }
            });
            RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

            // NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(); //dtls -> some sort of encryption
            DodgeGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        }catch(LobbyServiceException e){
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke(this , System.EventArgs.Empty);
        }
    }

    public async void QuickJoinLobby(){
        OnLobbyJoin?.Invoke(this , System.EventArgs.Empty);
        try{
            var availableLobbies = await LobbyService.Instance.QueryLobbiesAsync();
            if (availableLobbies.Results.Count == 0)
            {
                Debug.Log("No open lobbies found.");
            }

            joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = joinedLobby.Data[RELAY_KEY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            DodgeGameMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
            OnLobbyJoinFailed?.Invoke(this , System.EventArgs.Empty);
        }
    }
    public async void JoinLobbyById(string lobbyId){
        OnLobbyJoin?.Invoke(this , System.EventArgs.Empty);
        try{
            joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string relayJoinCode = joinedLobby.Data[RELAY_KEY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            DodgeGameMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
            OnLobbyJoinFailed?.Invoke(this , System.EventArgs.Empty);

        }
    }
    public async void JoinLobbyByCode(string lobbyCode){
        OnLobbyJoin?.Invoke(this , System.EventArgs.Empty);
        try{
            joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            
            string relayJoinCode = joinedLobby.Data[RELAY_KEY_JOIN_CODE].Value;
            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);
            RelayServerData relayServerData = joinAllocation.ToRelayServerData("dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            
            DodgeGameMultiplayer.Instance.StartClient();
        }catch(LobbyServiceException e){
            Debug.Log(e);
            OnLobbyJoinFailed?.Invoke(this , System.EventArgs.Empty);

        }
    }
    private async void GetLobbiesList(){
        try{
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions{
                Filters = new List<QueryFilter>{
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots ,"0",QueryFilter.OpOptions.GT)
                }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);
            OnLobbyListChanged?.Invoke(this , new OnLobbyListChangedEventArgs{
                lobbyList =  queryResponse.Results
            });
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    public Lobby GetLobby(){
        return joinedLobby;
    }
    public async void LeavePlayerFromLobby(){
        try{
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id , AuthenticationService.Instance.PlayerId);

            joinedLobby = null;
        }catch(LobbyServiceException e){
            Debug.Log(e);
        }
    }
    public async void DeleteLobby(){
        if(joinedLobby != null){
            try{
                await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);

                joinedLobby = null;
            }catch(LobbyServiceException e){
                Debug.Log(e);
            }
        }
    }
    // private async void OnApplicationQuit(){
    //     Debug.Log("Application is quitting. Removing player from lobby...");
        
    //     if (joinedLobby != null)
    //     {
    //         try
    //         {
    //             await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
    //             Debug.Log("Player successfully removed from lobby on quit.");
    //         }
    //         catch (LobbyServiceException e)
    //         {
    //             Debug.LogError("Error removing player from lobby on quit: " + e.Message);
    //         }
    //     }
    // }


}
