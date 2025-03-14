using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DodgeGameMultiplayer : NetworkBehaviour
{
    public const int MAX_PLAYER_COUNT = 4;
    private const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";

    public static DodgeGameMultiplayer Instance {get;private set;}
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;
    public event EventHandler OnPlayerDataListNetworkChanged;
    public event EventHandler OnSessionDisconnect;
    private NetworkList<PlayerData> playerDataNetworkList;
    [SerializeField]private List<GameObject> playerGameObjectList;
    private string playerName;

    private void Awake(){
        Instance = this;
        playerDataNetworkList = new();
        playerDataNetworkList.OnListChanged += PlayerDataNetwork_OnListChanged;
        DontDestroyOnLoad(gameObject);


        playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER , "PlayerName"+ UnityEngine.Random.Range(100,1000));
    }
    public string GetPlayerName(){
        return playerName;
    }

    public void SetPlayerName(string playerName){
        this.playerName = playerName;
        PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER , playerName);
    }
    private void PlayerDataNetwork_OnListChanged(NetworkListEvent<PlayerData> changeEvent){
        OnPlayerDataListNetworkChanged?.Invoke(this , System.EventArgs.Empty);
    }

    public void StartHost(){
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallBack;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_ClientOnClientDisconnect;
        NetworkManager.Singleton.StartHost();
    }


    private void NetworkManager_ClientOnClientDisconnect(ulong clientId){
        // if(clientId == NetworkManager.ServerClientId){
        //     ServerDisconnectedClientRpc();
        //     // OnSessionDisconnect?.Invoke(this , System.EventArgs.Empty);
        //     return;
        // }
        for(int i = 0;i<playerDataNetworkList.Count;i++){
            if(playerDataNetworkList[i].clientId == clientId){
                Debug.Log(playerDataNetworkList[i].clientId +"got disconnected : index = "+i);
                playerDataNetworkList.RemoveAt(i);
                // OnSessionDisconnect?.Invoke(this , System.EventArgs.Empty);
            }
        } 
        Debug.Log("connected clients = "+NetworkManager.Singleton.ConnectedClientsList.Count +"list count :"+playerDataNetworkList.Count);
        // DodgeGameLobby.Instance.LeavePlayerFromLobby();
    }
    // [ClientRpc]
    // private void ServerDisconnectedClientRpc(){
    //     Debug.Log("Server diconnceted");
    //     OnSessionDisconnect?.Invoke(this , System.EventArgs.Empty);
    // }

    private void NetworkManager_OnClientConnected(ulong clientId){
        playerDataNetworkList.Add(new PlayerData{
            clientId = clientId,
            playerCharacterVisualIndex = GetUniqueCharacterVisualIndexId()

        });
        SetPlayerNameServerRpc(GetPlayerName());
    }
    private int GetUniqueCharacterVisualIndexId(){
        for(int i = 0;i<4;i++){
            if(!PlayerContainsCharacterVisualIndex(i)){
                return i;
            }
        }
        return -1;
    }
    private bool PlayerContainsCharacterVisualIndex(int visualIndex){
        foreach(PlayerData playerData in playerDataNetworkList){
            if (playerData.playerCharacterVisualIndex == visualIndex){
                return true;
            }        
        }
        return false;
    }
    public void StartClient(){
        OnTryingToJoinGame?.Invoke(this , System.EventArgs.Empty);

        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnected;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_ClientOnClientConnected;
        // NetworkManager.Singleton.OnServerStopped += NetworkManager_OnServerStopped;
        NetworkManager.Singleton.StartClient();
    }
    private void NetworkManager_ClientOnClientConnected(ulong clientId){
        SetPlayerNameServerRpc(GetPlayerName());
    }   
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerNameServerRpc(string playerName , ServerRpcParams serverRpcParams = default){
        int playerIndex = GetPlayerDataIndexFromPlayerClientId(serverRpcParams.Receive.SenderClientId);
        PlayerData playerData = playerDataNetworkList[playerIndex];

        playerData.playerName = playerName;
        playerDataNetworkList[playerIndex] = playerData;  
    }
    private void NetworkManager_OnClientDisconnected(ulong clientId){
        OnFailedToJoinGame?.Invoke(this , System.EventArgs.Empty);
    }
    private void NetworkManager_ConnectionApprovalCallBack(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest ,NetworkManager.ConnectionApprovalResponse connectionApprovalResponse){
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString()){
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game has already started";
            return;
        }
        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_COUNT){
            connectionApprovalResponse.Approved = false;
            connectionApprovalResponse.Reason = "Game is Full";
            return;
        }
        connectionApprovalResponse.Approved = true;
    }

    public void DestroyBallObject(BaseThrowBall baseThrowBall)
    {
        if (baseThrowBall == null || baseThrowBall.NetworkObject == null)
        {
            Debug.LogError("Trying to destroy a ball that is not spawned or has already been destroyed!");
            return;
        }
        if (!baseThrowBall.NetworkObject.IsSpawned)
        {
            Debug.LogError("Network object is not spawned.");
            return;
        }

        Debug.Log("Server trying to destroy the ball");
        DestroyBallNetworkObjectServerRpc(baseThrowBall.NetworkObject);
    }


    [ServerRpc(RequireOwnership =false)]
    private void DestroyBallNetworkObjectServerRpc(NetworkObjectReference ballNetworkObjectReference){
        ballNetworkObjectReference.TryGet(out NetworkObject ballNetworkObject);
        
        if(ballNetworkObject == null) return;

        BaseThrowBall baseThrowBall = ballNetworkObject.GetComponent<BaseThrowBall>();
        baseThrowBall.DestroySelf();
        // DestroyBallNetworkObjectClientRpc(ballNetworkObjectReference);
    }
    // [ClientRpc]
    // private void DestroyBallNetworkObjectClientRpc(NetworkObjectReference ballNetworkObjectReference){
    // }

    public bool IsPlayerIndexConnected(int playerIndex){
        return playerIndex < playerDataNetworkList.Count;
    }

    public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex){
        return playerDataNetworkList[playerIndex];
    }

    public PlayerData GetPlayerDataFromClientId(ulong clientId){
        foreach(PlayerData playerData in playerDataNetworkList){
            if (playerData.clientId == clientId){
                return playerData;
            }
        }
        return default;
    }
    public int GetPlayerDataIndexFromPlayerClientId(ulong clientId){
        for(int i = 0;i<playerDataNetworkList.Count;i++){
            if(playerDataNetworkList[i].clientId == clientId){
                return i;
            }
        }

        return -1;
    }
    public Transform GetPlayerVisualTransformObject(int playerVisualIndex){
        return playerGameObjectList[playerVisualIndex].transform;
    }
    public void ChangeCharacterVisual(int playerVisualCharacterIndex){
        ulong clientId = NetworkManager.Singleton.LocalClientId;
        ChangeCharacterVisualServerRpc(playerVisualCharacterIndex , clientId);
    }
    [ServerRpc(RequireOwnership =false)]
    private void ChangeCharacterVisualServerRpc(int playerVisualCharacterIndex , ulong clientId){
        int playerIndex = GetPlayerDataIndexFromPlayerClientId(clientId);
        PlayerData playerData = playerDataNetworkList[playerIndex];
        playerData.playerCharacterVisualIndex = playerVisualCharacterIndex;

        playerDataNetworkList[playerIndex] = playerData;
    }
    [ServerRpc(RequireOwnership =false)]
    public void AddTeamIdToPlayerServerRpc(ulong clientId , int teamId){
        // AddTeamIdToPlayerServerRpc(teamId , playerIndex);
        // AddTeamIdToPlayerClientRpc(teamId , playerIndex);
        int playerIndex = GetPlayerDataIndexFromPlayerClientId(clientId);
        PlayerData playerData = GetPlayerDataFromClientId(clientId);
        playerData.teamId = teamId;
        playerDataNetworkList[playerIndex] = playerData;

        Debug.Log("client id :"+playerDataNetworkList[playerIndex].clientId +" team id: "+playerDataNetworkList[playerIndex].teamId);
    }

}
