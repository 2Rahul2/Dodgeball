using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
    private Lobby lobby;
    private void Awake(){
        GetComponent<Button>().onClick.AddListener(()=>{
            if(lobby!=null){
                DodgeGameLobby.Instance.JoinLobbyById(lobby.Id);
            }
        });
    }
    public void SetLobby(Lobby lobby){
        this.lobby = lobby;
        if(transform.childCount!=0){
            transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
        }
    }
}
