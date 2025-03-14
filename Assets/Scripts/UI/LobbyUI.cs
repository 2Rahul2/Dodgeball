using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]private Button mainMenuButton;
    [SerializeField]private Button createLobbyButton;
    [SerializeField]private Button quickJoinButton;

    [SerializeField]private CreateLobbyUI createLobbyUI;

    [SerializeField]private Button joinByCodeButton;
    [SerializeField]private TMP_InputField codeInput;
    [SerializeField]private TMP_InputField playerNameInput;

    [SerializeField]private Transform lobbyTemplate;
    [SerializeField]private Transform lobbyContainer;



    private void Awake(){
        mainMenuButton.onClick.AddListener(()=>{
            Loader.Load(Loader.Scene.MainMenuScene);
        });
        createLobbyButton.onClick.AddListener(()=>{
            createLobbyUI.Show();
            // DodgeGameLobby.Instance.CreateLobby("lobby name", false);
        });
        quickJoinButton.onClick.AddListener(()=>{
            DodgeGameLobby.Instance.QuickJoinLobby();
        });

        joinByCodeButton.onClick.AddListener(()=>{
            DodgeGameLobby.Instance.JoinLobbyByCode(codeInput.text);
        });
        lobbyTemplate.gameObject.SetActive(false);
    }
    private void Start(){
        playerNameInput.text = DodgeGameMultiplayer.Instance.GetPlayerName();
        playerNameInput.onValueChanged.AddListener((string newNameText)=>{
            DodgeGameMultiplayer.Instance.SetPlayerName(newNameText);
        });

        DodgeGameLobby.Instance.OnLobbyListChanged += DodgeGameLobby_LobbyListChanged;
        UpdateList(new List<Lobby>());
    }
        
    private void DodgeGameLobby_LobbyListChanged(object sender ,DodgeGameLobby.OnLobbyListChangedEventArgs e){
        UpdateList(e.lobbyList);
    }
    private void UpdateList(List<Lobby> lobbyList){
        foreach(Transform child in lobbyContainer){
            if(child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(Lobby lobby in lobbyList){
            Transform lobbyTransform =  Instantiate(lobbyTemplate ,lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);    
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);  
        }
    }
    private void OnDestroy(){
        DodgeGameLobby.Instance.OnLobbyListChanged -= DodgeGameLobby_LobbyListChanged;
    }
}
