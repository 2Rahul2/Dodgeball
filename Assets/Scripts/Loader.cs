using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    public enum Scene{
        MainMenuScene,
        LobbyScene,
        CharacterSelectScene,
        SampleScene,
    }
    public static void Load(Scene scene){
        SceneManager.LoadScene(scene.ToString());
    }
    public static void LoadNetwork(Scene targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString() , LoadSceneMode.Single);
    }
    public static void LoadNetworkMap(MapSelectorUI.MapName targetScene){
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString() , LoadSceneMode.Single);
    }
}
