using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSelectReady : NetworkBehaviour
{
    private const int MIN_PLAYERS = 2;
    private const int MAX_PLAYERS = 4;
    public static CharacterSelectReady Instance {get;private set;}
    private Dictionary<ulong , bool> playerReadyDictionary;
    public event EventHandler OnDictionaryChanged;

    [SerializeField]private Transform loadingBackground;
    private void Awake(){
        Instance = this;
        playerReadyDictionary = new();
    } 
    public void SetPlayerReady(){
        SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;
        bool allPlayersReady = true;
        foreach(ulong clientId  in NetworkManager.Singleton.ConnectedClientsIds){
            if(!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId]){
                allPlayersReady = false;
                break;
            }
        }
        int totalPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
        if(allPlayersReady && (totalPlayers == MIN_PLAYERS || totalPlayers == MAX_PLAYERS)){
            EnableLoadingScreenClientRpc();
            DodgeGameLobby.Instance.DeleteLobby();
            AssignTeam();
            Loader.LoadNetworkMap(MapSelectorUI.Instance.GetMapName());
        }
    }
    [ClientRpc]
    private void EnableLoadingScreenClientRpc(){
        loadingBackground.gameObject.SetActive(true);
    }
    private void AssignTeam(){
        int totalPlayers = NetworkManager.Singleton.ConnectedClientsIds.Count;
        int teamSize = totalPlayers / 2;
        int index = 0;
   
        Debug.Log("total Players : "+totalPlayers +" team size : "+teamSize);

        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
            int playerTeamId = index < teamSize ?1 :2;
            Debug.Log("team id : "+playerTeamId);

            DodgeGameMultiplayer.Instance.AddTeamIdToPlayerServerRpc(clientId , playerTeamId);
            index++;
        }
    }
    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId){
        playerReadyDictionary[clientId] = true;
        OnDictionaryChanged?.Invoke(this , System.EventArgs.Empty);
    }

    public bool IsPlayerReady(ulong clientId){
        return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
    }

}
