using System;
using TMPro;
using UnityEngine;

public class CharacterSelectPlayer : MonoBehaviour
{
    [SerializeField]private int playerIndex;
    // [SerializeField]private int playerCharacterVisualIndex;
    [SerializeField]private GameObject readyGameObject;
    [SerializeField]private TextMeshPro playerNameText;
    [SerializeField]private PlayerCharacterVisual playerCharacterVisual;

    private void Start(){
        DodgeGameMultiplayer.Instance.OnPlayerDataListNetworkChanged += DodgeGameMultiplayer_OnPlayerDataListNetworkChanged;
        CharacterSelectReady.Instance.OnDictionaryChanged += CharacterSelectReady_OnDictionaryPlayerReadyChanged;
        UpdatePlayer();
    }
    private void CharacterSelectReady_OnDictionaryPlayerReadyChanged(object sender , System.EventArgs e){
        UpdatePlayer();
    }   
    private void DodgeGameMultiplayer_OnPlayerDataListNetworkChanged(object sender , System.EventArgs e){
        UpdatePlayer();
    }
    private void UpdatePlayer(){
        if(DodgeGameMultiplayer.Instance.IsPlayerIndexConnected(playerIndex)){
            Show();
            PlayerData playerData = DodgeGameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));
            playerNameText.text = playerData.playerName.ToString();
            playerCharacterVisual.SetPlayerCharacterVisual(playerData.playerCharacterVisualIndex);
        }else{
            Hide();
        }
    }
    private void Show(){
        gameObject.SetActive(true);
    }
    private void Hide(){
        gameObject.SetActive(false);
    }
    void OnDestroy()
    {
     DodgeGameMultiplayer.Instance.OnPlayerDataListNetworkChanged -= DodgeGameMultiplayer_OnPlayerDataListNetworkChanged;
        CharacterSelectReady.Instance.OnDictionaryChanged -= CharacterSelectReady_OnDictionaryPlayerReadyChanged;   
    }

}
