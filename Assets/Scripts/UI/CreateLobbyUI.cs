using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateLobbyUI : MonoBehaviour
{
    [SerializeField]private Button closeButton;
    [SerializeField]private Button createPublicButton;
    [SerializeField]private Button createPrivateButton;
    [SerializeField]private TMP_InputField lobbyNameInput;
    private void Awake(){
        createPublicButton.onClick.AddListener(()=>{
            DodgeGameLobby.Instance.CreateLobby(lobbyNameInput.text , false);
        });

        createPrivateButton.onClick.AddListener(()=>{
            if(lobbyNameInput.text == ""){
                lobbyNameInput.text = "lobby#"+Random.Range(1 , 1000).ToString();
            }
            DodgeGameLobby.Instance.CreateLobby(lobbyNameInput.text , true);
        });
        closeButton.onClick.AddListener(()=>{
            Hide();
        });
    }

    private void Start(){
        Hide();
    }
    private void Hide(){
        gameObject.SetActive(false);
    }

    public void Show(){
        gameObject.SetActive(true);
    }

}
