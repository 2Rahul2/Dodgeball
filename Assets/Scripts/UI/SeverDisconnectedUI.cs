using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class SeverDisconnectedUI : MonoBehaviour
{
    [SerializeField]private Transform backgroundImage;
    [SerializeField]private Button mainMenuButton;

    private void Awake(){
        DodgeGameMultiplayer.Instance.OnSessionDisconnect += DodgeGameMultiplayer_SessionDisconnect;
        mainMenuButton.onClick.AddListener(()=>{
            MainMenuButton();
        });
        backgroundImage.gameObject.SetActive(false);
    }
    private void MainMenuButton(){
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }

    private void DodgeGameMultiplayer_SessionDisconnect(object sender , System.EventArgs e){
        Debug.Log("host got broomm broommm wooshhhhh");
        backgroundImage.gameObject.SetActive(true);
    }
}
