using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCalcUI : MonoBehaviour
{
    public static ScoreCalcUI Instance {get;private set;}
    [SerializeField]private Color redColor;
    [SerializeField]private Color blueColor;
    [SerializeField]private Image blueScoreImage;
    [SerializeField]private Image redScoreImage;
    private int myTeamId;

    [SerializeField]private Transform messageBox;
    [SerializeField]private TextMeshProUGUI matchResultmessage;

    [SerializeField]private Transform finalScoreMessageBox;
    [SerializeField]private TextMeshProUGUI finalScoreMessageText;
    [SerializeField]private Button mainMenuButton;
    private void Awake(){
        Instance = this;
        blueScoreImage.fillAmount = 0;
        redScoreImage.fillAmount = 0;
        HideFinalMessage();
        HideMessage();
    }

    private void DodgeGameManager_OnGameStateChanged(object sender, EventArgs e)
    {
        Debug.Log("current game state is === "+DodgeGameManager.Instance.GetGameState());
        if(DodgeGameManager.Instance.IsResultTimerState()){
            ShowMessage();
        }else{
            HideMessage();
        }
    }

    private void Start(){
        ScoreNetwork.Instance.ResetUI += ScoreNetworkUI_ResetUI;
        DodgeGameManager.Instance.OnGameStateChanged += DodgeGameManager_OnGameStateChanged;
        mainMenuButton.onClick.AddListener(()=>{
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
    private void ScoreNetworkUI_ResetUI(object sender , System.EventArgs e){
        HideMessage();
        matchResultmessage.text = "";
    }
    public void SetTeamId(int id){
        myTeamId = id;
    }
    private void SetBlueScore(){
        blueScoreImage.fillAmount += 0.2f;
        matchResultmessage.text = "Your Team Won The Round";
    }
    private void SetRedScore(){
        redScoreImage.fillAmount += 0.2f;
        matchResultmessage.text = "Opponent Team Won The Round";
    }

    public void SetScore(int teamID){
        ShowMessage();
        if(teamID == myTeamId){
            SetBlueScore();
        }else{
            SetRedScore();
        }
        // DodgeGameManager.Instance.SetGameStateClientRpc(DodgeGameManager.GameState.ResultTimer);
    }
    public void VictoryDefeatMessage(int teamId){
        HideMessage();
        if(teamId == myTeamId){
            SoundManager.Instance.PlayVictorySound();
            finalScoreMessageText.text = "Victory!!";
            finalScoreMessageText.color = blueColor;
        }else{
            SoundManager.Instance.PlayDefeatSound();
            finalScoreMessageText.text = "Defeat..";
            finalScoreMessageText.color = redColor;
        }
        ShowFinalMessage();
    }
    private void ShowMessage(){
        messageBox.gameObject.SetActive(true);
    }
    private void HideMessage(){
        messageBox.gameObject.SetActive(false);
    }

    private void ShowFinalMessage(){
        finalScoreMessageBox.gameObject.SetActive(true);
    }
    private void HideFinalMessage(){
        finalScoreMessageBox.gameObject.SetActive(false);
    }
    
}
