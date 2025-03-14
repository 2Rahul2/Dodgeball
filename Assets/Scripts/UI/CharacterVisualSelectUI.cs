using UnityEngine;
using UnityEngine.UI;

public class CharacterVisualSelectUI : MonoBehaviour
{
    [SerializeField]private int characterIndex_Id;
    private void Start(){
        GetComponent<Button>().onClick.AddListener(()=>{
            DodgeGameMultiplayer.Instance.ChangeCharacterVisual(characterIndex_Id);
        });
    }
}
