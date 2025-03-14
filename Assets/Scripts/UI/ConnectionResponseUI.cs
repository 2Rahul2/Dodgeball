using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionResponseUI : MonoBehaviour
{
    [SerializeField]private TextMeshProUGUI messageText;
    [SerializeField]private Button closeButton;
    // [SerializeField]private Transform loadingScreen;
    private void Awake(){
        closeButton.onClick.AddListener(()=>{
            Hide();
        });
    }

    private void Start(){
        DodgeGameMultiplayer.Instance.OnFailedToJoinGame += ShowFailedMessage;
        DodgeGameLobby.Instance.OnCreateLobby += DodgeGameLobby_OnCreateLobby;
        DodgeGameLobby.Instance.OnCreateLobbyFailed += DodgeGameLobby_OnFailedLobby;

        DodgeGameLobby.Instance.OnLobbyJoin += DodgeGameLobby_OnLobbyJoin;
        DodgeGameLobby.Instance.OnLobbyJoinFailed += DodgeGameLobby_OnLobbyJoinFailed;

        Hide();
    }
    private void DodgeGameLobby_OnLobbyJoin(object sender , System.EventArgs e){
        ShowMessage("Joining Lobby...");
    }
    private void DodgeGameLobby_OnLobbyJoinFailed(object sender , System.EventArgs e){
        ShowMessage("Failed to join Lobby");
        // loadingScreen.gameObject.SetActive(false);
    }
    private void DodgeGameLobby_OnCreateLobby(object sender , System.EventArgs e){
        ShowMessage("Creating Lobby...");
    }
    private void DodgeGameLobby_OnFailedLobby(object sender , System.EventArgs e){
        ShowMessage("Failed to create lobby.");
        // loadingScreen.gameObject.SetActive(false);
    }
    private void ShowFailedMessage(object sender , System.EventArgs e){
        if(NetworkManager.Singleton.DisconnectReason == ""){
            ShowMessage("Failed");
        }else{
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }
    private void ShowMessage(string message){
        Show();
        messageText.text = message;
    }
    void Hide(){
        gameObject.SetActive(false);
    }
    void Show(){
        gameObject.SetActive(true);
    }
    private void OnDestroy(){
        DodgeGameMultiplayer.Instance.OnFailedToJoinGame -= ShowFailedMessage;
        DodgeGameLobby.Instance.OnCreateLobby -= DodgeGameLobby_OnCreateLobby;
        DodgeGameLobby.Instance.OnCreateLobbyFailed -= DodgeGameLobby_OnFailedLobby;

        DodgeGameLobby.Instance.OnLobbyJoin -= DodgeGameLobby_OnLobbyJoin;
        DodgeGameLobby.Instance.OnLobbyJoinFailed -= DodgeGameLobby_OnLobbyJoinFailed;
    }
}
