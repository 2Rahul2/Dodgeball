using UnityEngine;
using UnityEngine.UI;

public class TestingCharacterSelectUI : MonoBehaviour
{
    [SerializeField]private Button readyButton;
    private void Start(){
        readyButton.onClick.AddListener(()=>{
            CharacterSelectReady.Instance.SetPlayerReady();
        });
    }
}
