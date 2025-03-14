using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectUI : MonoBehaviour
{
    [SerializeField]private Button mainMenuButton;
    [SerializeField]private Button readyButton;
    [SerializeField]private TextMeshProUGUI lobbyNameText;
    [SerializeField]private TextMeshProUGUI lobbyCodeText;

    private void Awake(){
        mainMenuButton.onClick.AddListener(()=>{
            NetworkManager.Singleton.Shutdown();    
            DodgeGameLobby.Instance.LeavePlayerFromLobby();
            Loader.Load(Loader.Scene.MainMenuScene);
        });

        readyButton.onClick.AddListener(()=>{
            CharacterSelectReady.Instance.SetPlayerReady();
        });

    }
    private void Start(){
        DisplayLobbyNameAndCode();
    }
    private void DisplayLobbyNameAndCode(){
        Lobby lobby = DodgeGameLobby.Instance.GetLobby();
        lobbyNameText.text = "Lobby Name : " +lobby.Name;
        lobbyCodeText.text = "Code : "+lobby.LobbyCode;
    }
}
