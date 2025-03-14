using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DodgeGameManager : NetworkBehaviour
{
    private const string OPPONENT_LAYER_NAME = "Opponent";
    private const string PLAYER_LAYER_NAME = "Player";

    public static DodgeGameManager Instance {get;private set;}
    public event EventHandler OnGameStateChanged;
    private Dictionary<ulong, bool> playerReadyState;
    // private Dictionary<int , List<ulong>> playerTeamState;

    private enum State{
        WaitingForPlayer,
        StartGame
    }
    public enum GameState{
        StartGame,
        WaitTimer,
        ResultTimer,
        GameOver,
    };
    private NetworkVariable<GameState> gameState = new NetworkVariable<GameState>(GameState.WaitTimer);
    private NetworkVariable<float> resetWaitTimer = new(3f);
    private NetworkVariable<float> waitTimer = new(3f);

    private NetworkVariable<float> resetResultTimer = new(2f);
    private NetworkVariable<float> resultTimer = new(2f);
    public event EventHandler OnGameStarted;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.WaitingForPlayer);
    [SerializeField]private Transform playerPrefab;


    private void Awake(){
        Instance = this;
        playerReadyState = new Dictionary<ulong, bool>();
        // playerTeamState = new Dictionary<int, List<ulong>>();
    }
    public Transform teamOnePosition;
    public Transform teamTwoPosition;


    private void Update(){
        if(!IsServer)return;

        switch(gameState.Value){
            case GameState.StartGame:
                break;
            case GameState.WaitTimer:
                waitTimer.Value -= Time.deltaTime;
                if(waitTimer.Value < 0f){
                    waitTimer.Value = resetWaitTimer.Value;
                    gameState.Value = GameState.StartGame;
                }
                break;
            case GameState.ResultTimer:
                resultTimer.Value -= Time.deltaTime;
                if (resultTimer.Value < 0f){
                    resultTimer.Value = resetResultTimer.Value;
                    gameState.Value = GameState.WaitTimer;
                }
                break;
            case GameState.GameOver:
                break;
        }
    }
    public override void OnNetworkSpawn(){
        if(IsServer){
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnCompleted;
        }
        state.OnValueChanged += GameManager_OnStateValueChanged;
        gameState.OnValueChanged += GameManager_OnGameStateValueChanged;
        SetPlayerReadyServerRpc();
    }

    private void GameManager_OnGameStateValueChanged(GameState previousValue, GameState newValue){
        OnGameStateChanged?.Invoke(this , System.EventArgs.Empty);
    }

    private void GameManager_OnStateValueChanged(State previousValue ,State newValue){
        Debug.Log("on state event is executed | old state = " + previousValue.ToString()+ " new State = "+newValue.ToString());
        
        if(newValue == State.StartGame){
            Debug.Log("start game event is executing");
            OnGameStarted?.Invoke(this , System.EventArgs.Empty);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default){
        playerReadyState[serverRpcParams.Receive.SenderClientId] = true;
        bool allPlayersReady = true;
        foreach(ulong playerId in NetworkManager.Singleton.ConnectedClientsIds){
            if(!playerReadyState.ContainsKey(playerId) || !playerReadyState[playerId]){
                allPlayersReady = false;
                break;
            }
        }

        if (allPlayersReady){
            Debug.Log("game state as changed to start game");
            state.Value = State.StartGame;
        }
    }

    private void SceneManager_OnCompleted(string sceneName , UnityEngine.SceneManagement.LoadSceneMode loadSceneMode , List<ulong> clientsCompleted, List<ulong> clientsTimedOut){
        Debug.Log("Spawning Players");
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
            Debug.Log("Spawning Players "+clientId.ToString());
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId , true);
            // playerTransform.localScale = new Vector3(1.5f ,1.5f ,1.5f);
            // PlayerData currentPlayerData = DodgeGameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
            // if(currentPlayerData.teamId == 1){
            //     playerTransform.position = new Vector3(UnityEngine.Random.Range(teamOnePosition.position.x ,teamOnePosition.position.x + 5 ), teamOnePosition.position.y , UnityEngine.Random.Range(teamOnePosition.position.z ,teamOnePosition.position.z + 5 ));
            // }else{
            //     playerTransform.position = new Vector3(UnityEngine.Random.Range(teamTwoPosition.position.x ,teamTwoPosition.position.x + 5 ), teamTwoPosition.position.y , UnityEngine.Random.Range(teamTwoPosition.position.z ,teamTwoPosition.position.z + 5 ));
            // }
        }
        // AssignTeamServerRpc();
        StartCoroutine(BallSpawnManager.Instance.StartSpawning());
        SetMarkerForPlayersServerRpc();
    }
    [ServerRpc]
    private void AssignTeamServerRpc(){
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
        // StartCoroutine(SetMarkersAfterDelay());
        // SetMarkerForPlayersClientRpc();
    }

    public Transform GetPlayerObjectTransformWithNetworkClientId(ulong clientId){
        if(NetworkManager.Singleton.ConnectedClients.ContainsKey(clientId)){
            Debug.Log("player is inside the network , getting its transform");
            return NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.transform;
        }
        Debug.Log("no player in network");
        return null;
    }
    [ServerRpc]
    private void SetMarkerForPlayersServerRpc(){
        SetMarkerForPlayersClientRpc();
        SetSelfPlayerToDisableControlClientRpc();
    }
    [ClientRpc]
    private void SetSelfPlayerToDisableControlClientRpc(){
        DisablePlayerControls.Instance.SetPlayerObject(GetPlayerObjectTransformWithNetworkClientId(NetworkManager.Singleton.LocalClientId));
    }
    [ClientRpc]
    public void SetMarkerForPlayersClientRpc(){
        PlayerData playerData = DodgeGameMultiplayer.Instance.GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
        Debug.Log("setting marker visuals");
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
            PlayerData otherPlayerData = DodgeGameMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
            Debug.Log("marker client id : " +clientId +"team id : "+otherPlayerData.teamId);
            Transform playerTransform = GetPlayerObjectTransformWithNetworkClientId(clientId);
            playerTransform.GetComponent<Player>().SetTeamId(otherPlayerData.teamId);
            if(playerData.teamId == otherPlayerData.teamId){
                ScoreCalcUI.Instance.SetTeamId(otherPlayerData.teamId);
                if(playerTransform!=null){
                    int layerIndex = LayerMask.NameToLayer(OPPONENT_LAYER_NAME);
                    playerTransform.gameObject.layer = layerIndex;
                    TeamHighLightVisual.Instance.BlueMarkTeamMatesVisual(playerTransform);
                }
            }else{
                if(playerTransform!=null){
                    int layerIndex = LayerMask.NameToLayer(OPPONENT_LAYER_NAME);
                    playerTransform.gameObject.layer = layerIndex;
                    TeamHighLightVisual.Instance.RedMarkTeamMatesVisual(playerTransform);
                }
            }
        }
    }

    public float GetCountDownTimer(){
        return waitTimer.Value;
    }
    public bool IsCountDownTimer(){
        return GameState.WaitTimer == gameState.Value;
    }
    public bool IsResultTimerState(){
        return GameState.ResultTimer == gameState.Value;
    }

    public void SetGameState(GameState state){
        gameState.Value = state;
    }
    public string GetGameState(){
        return gameState.Value.ToString();
    }
}

