using System;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour
{
    [SerializeField]private Transform backgroundImage;
    void Start()
    {
        backgroundImage.gameObject.SetActive(true);
        DodgeGameManager.Instance.OnGameStarted += DodgeGameManager_GameCanStart;
    }
    private void DodgeGameManager_GameCanStart(object sender , System.EventArgs e){
        Debug.Log("game is ready to begin");
        backgroundImage.gameObject.SetActive(false);
        // DodgeGameManager.Instance.SetMarkerForPlayers();
    }
     
}
