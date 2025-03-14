using Unity.Netcode;
using UnityEngine;

public class DisablePlayerControls : MonoBehaviour
{
    private Transform playerObject;
    public static DisablePlayerControls Instance;
    private void Awake(){
        Instance = this;
    }
    public void SetPlayerObject(Transform playerObject){
        this.playerObject = playerObject;
    }
    public void TogglePlayerScript(bool isEnabled){
        Debug.Log("gotta toggle player object");
        if(playerObject!=null){
            Debug.Log("toggling player object");
            // playerObject.gameObject.SetActive(isEnabled);
            // playerObject.GetComponent<Player>().enabled = isEnabled;
            if(isEnabled){
                playerObject.GetComponent<Player>().SetGotHitServerRpc(false);
            }
        }
    }
}
