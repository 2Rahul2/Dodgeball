using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class ScoreNetwork : NetworkBehaviour
{
    public static ScoreNetwork Instance {get;private set;}
    

    private const int TEAM_ONE_ID = 1;
    private const int TEAM_TWO_ID = 2;
    private const int TOTAL_ROUND = 5;
    private int winningScore = 1;

    // private NetworkVariable<int> teamOneScore = new();
    // private NetworkVariable<int> teamTwoScore = new();

    public event EventHandler ResetUI;
    public event EventHandler<OnPlayerGotHit_PlayerId> OnPlayerGotHit_DisablePlayer;
    public class OnPlayerGotHit_PlayerId{
        public ulong clientId;
        public bool isEnable;
    }

    private NetworkList<TeamScore> teamScore;
    // private NetworkList<TeamScore> teamTwoScore;


    // private NetworkVariable<Dictionary<int , int>> teamScore = new();
    private void Awake(){
        Instance = this;
        teamScore = new NetworkList<TeamScore>();
    }
    private void Start(){
        DodgeGameManager.Instance.OnGameStateChanged += DodgeGameManger_OnGameStateChanged;
    }

    private void DodgeGameManger_OnGameStateChanged(object sender, EventArgs e)
    {
        if(DodgeGameManager.GameState.StartGame.ToString() == DodgeGameManager.Instance.GetGameState()){
            ResetPlayerControls();
        }
    }

    public override void OnNetworkSpawn(){
        if(IsServer){
            Debug.Log("adding team score team id");
            teamScore.Add(new TeamScore(0 ,0 ,1));
            teamScore.Add(new TeamScore(0 ,0 ,2));
            // ResetPlayerScoreServerRpc();
            winningScore = NetworkManager.Singleton.ConnectedClientsIds.Count / 2;
            Debug.Log("SERVER : winning score is : "+winningScore);

        }
        Debug.Log("winning score is : "+winningScore);
        teamScore.OnListChanged += ScoreNetwork_CheckWinStatus;
    }
    private void ScoreNetwork_CheckWinStatus(NetworkListEvent<TeamScore> changeEvent)
    {
        Debug.Log("changed occured in team network list");
        CheckRoundWonStatusServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void CheckRoundWonStatusServerRpc(){
        Debug.Log("length of team score is : "+teamScore.Count);
        for(int i = 0;i < teamScore.Count;i++){
            Debug.Log("team id : "+teamScore[i].teamId +" won these round : "+teamScore[i].roundWon);
            if(teamScore[i].roundWon >= TOTAL_ROUND){
                Debug.Log("VICTORYY FOR  Team id : "+teamScore[i].teamId);
                // send defeat and victory message
                // SetVisualScoreClientRpc(teamScore[i].teamId);
                DodgeGameManager.Instance.SetGameState(DodgeGameManager.GameState.GameOver);
                SetFinalScoreClientRpc(teamScore[i].teamId);
                return;
            }
        }

    }
    [ServerRpc(RequireOwnership =false)]
    private void ResetPlayerScoreServerRpc(){
        for(int i = 0;i <teamScore.Count ; i++){
            teamScore[i] = new TeamScore(0 ,teamScore[i].roundWon , teamScore[i].teamId);
        }
        // ResetPlayerControlsClientRpc();
    } 
    private void ResetPlayerControls(){
        SetAllPlayerControlsDiabledEnabledClientRpc(true);
        // DisablePlayerControls.Instance.TogglePlayerScript(true);
        // DodgeGameMultiplayer.Instance.getpla
    }

    [ServerRpc(RequireOwnership =false)]
    public void DecidePlayerWinServerRpc(int playerTeamId ,ulong opponentClientId){
        Debug.Log("which player going to win");
        for(int i =0;i<teamScore.Count;i++){
            Debug.Log("teams in match : "+teamScore[i].teamId);
            if(teamScore[i].teamId == playerTeamId){
                int totalScore = teamScore[i].score + 1;
                int roundWon = teamScore[i].roundWon;
                if (totalScore >= winningScore){
                    // reset score 
                    roundWon ++;
                    TeamScore newTeamScore = new TeamScore(totalScore , roundWon ,playerTeamId);
                    teamScore[i] = newTeamScore;
                    ResetPlayerScoreServerRpc();
                    SetVisualScoreClientRpc(teamScore[i].teamId);
                    SetAllPlayerControlsDiabledEnabledClientRpc(false);
                    Debug.Log("Team id : "+teamScore[i].teamId + " wins the match , round won : "+ roundWon);
                    // send ui to all users;
                }else{
                    SetControlsDisabledClientRpc(opponentClientId);
                    TeamScore newTeamScore = new TeamScore(totalScore , roundWon ,playerTeamId);
                    teamScore[i] = newTeamScore;
                }
                // CheckRoundWonStatusServerRpc();
                break;
            }
        }
    }
    [ClientRpc]
    private void SetVisualScoreClientRpc(int win_TeamId){
        ScoreCalcUI.Instance.SetScore(win_TeamId);
        if(IsServer){
            DodgeGameManager.Instance.SetGameState(DodgeGameManager.GameState.ResultTimer);
        }

    }
    [ClientRpc]
    private void SetAllPlayerControlsDiabledEnabledClientRpc(bool isEnable){
        Debug.Log("calling toggle function , client id matched");
        foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds){
            OnPlayerGotHit_DisablePlayer?.Invoke(this , new OnPlayerGotHit_PlayerId{
                clientId = clientId,
                isEnable = isEnable 
            });
        }
    }
    [ClientRpc]
    private void SetControlsDisabledClientRpc(ulong toDisableControlClientId){
        Debug.Log("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
        OnPlayerGotHit_DisablePlayer?.Invoke(this , new OnPlayerGotHit_PlayerId{
            clientId = toDisableControlClientId,
            isEnable = false 
        });
        // if(NetworkManager.Singleton.LocalClientId == toDisableControlClientId){
        //     Debug.Log("calling toggle function , client id matched");
        //     DisablePlayerControls.Instance.TogglePlayerScript(false);
        // }
    }
    [ClientRpc]
    private void SetFinalScoreClientRpc(int win_TeamId){
        ScoreCalcUI.Instance.VictoryDefeatMessage(win_TeamId);
    }
}

// [Serializable]
// public struct TeamScore : INetworkSerializable
// {
//     public int score;
//     public int roundWon;
//     public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
//     {
//         serializer.SerializeValue(ref score);
//         serializer.SerializeValue(ref roundWon);
//     }

//     // public bool Equals(TeamScore other)
//     // {
//     //     return score == other.score && roundWon == other.roundWon;
//     // }

//     public TeamScore(int score , int roundWon){
//         this.score = score;
//         this.roundWon = roundWon;
//     }
// }

