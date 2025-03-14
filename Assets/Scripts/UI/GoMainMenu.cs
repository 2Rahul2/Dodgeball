using UnityEngine;
using UnityEngine.UI;

public class GoMainMenu : MonoBehaviour
{
    [SerializeField]private Transform loadingBackground;
    [SerializeField]private Button mainMenu;

    private void Awake(){
        mainMenu.onClick.AddListener(()=>{
            loadingBackground.gameObject.SetActive(true);
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
}
