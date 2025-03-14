using UnityEngine;
using UnityEngine.UI;

public class QuitMainMenuButton : MonoBehaviour
{
    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(()=>{
            Loader.Load(Loader.Scene.MainMenuScene);
        });
    }
}
