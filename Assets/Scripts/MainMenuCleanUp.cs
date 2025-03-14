using Unity.Netcode;
using UnityEngine;

public class MainMenuCleanUp : MonoBehaviour
{
    private void Awake(){
        if(NetworkManager.Singleton != null){
            Destroy(NetworkManager.Singleton.gameObject);
        }
        if(DodgeGameMultiplayer.Instance != null){
            Destroy(DodgeGameMultiplayer.Instance.gameObject);
        }
        if(DodgeGameLobby.Instance!=null){
            Destroy(DodgeGameLobby.Instance.gameObject);
        }
    }
}
