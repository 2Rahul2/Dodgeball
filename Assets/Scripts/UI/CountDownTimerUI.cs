using System;
using TMPro;
using UnityEngine;

public class CountDownTimerUI : MonoBehaviour
{
    private const string POPUP = "PopUp";
    [SerializeField]private TextMeshProUGUI timerText;
    private int prevCountDownTimer;
    private void Awake(){
        Hide();
    }
    void Start()
    {
        DodgeGameManager.Instance.OnGameStateChanged += DodgeGameManager_GameStateChanged;
    }

    private void DodgeGameManager_GameStateChanged(object sender, EventArgs e){
        if(DodgeGameManager.Instance.IsCountDownTimer()){
            Show();
        }else{
            Hide();
        }
    }

    void Update(){
        int countDownTimer = Mathf.CeilToInt(DodgeGameManager.Instance.GetCountDownTimer());
        timerText.text = countDownTimer.ToString();
        if(prevCountDownTimer != countDownTimer){
            prevCountDownTimer = countDownTimer;
            timerText.GetComponent<Animator>().SetTrigger(POPUP);   
        }
    }

    private void Hide(){
        gameObject.SetActive(false);
    }

    private void Show(){
        gameObject.SetActive(true);
    }
}
