using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
    [SerializeField]private Button createGameButton;
    [SerializeField]private Button joinGameButton;
    private void Awake(){
        createGameButton.onClick.AddListener(()=>{
            DodgeGameMultiplayer.Instance.StartHost();
            Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
        });

        joinGameButton.onClick.AddListener(()=>{
            DodgeGameMultiplayer.Instance.StartClient();
        });
    }
}
