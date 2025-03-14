using UnityEngine;

public class ConnectingUI : MonoBehaviour
{
    private void Start(){
        DodgeGameMultiplayer.Instance.OnTryingToJoinGame += Show_ConnectingScreen;
        DodgeGameMultiplayer.Instance.OnFailedToJoinGame += Hide_ConnectingScreen;
        Hide();
    }
    private void Hide_ConnectingScreen(object sender , System.EventArgs e){
        Hide();
    }
    private void Show_ConnectingScreen(object sender , System.EventArgs e){
        Show();
    }
    void Hide(){
        gameObject.SetActive(false);
    }
    void Show(){
        gameObject.SetActive(true);
    }

    private void OnDestroy(){
        DodgeGameMultiplayer.Instance.OnTryingToJoinGame -= Show_ConnectingScreen;
        DodgeGameMultiplayer.Instance.OnFailedToJoinGame -= Hide_ConnectingScreen;
    }
}
