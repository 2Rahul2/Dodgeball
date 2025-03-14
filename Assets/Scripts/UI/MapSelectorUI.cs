using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectorUI : NetworkBehaviour
{
    public static MapSelectorUI Instance {get;private set;}
    // [SerializeField]private Transform mapSelector;
    // have selector parent as this current one
    public enum MapName{
        WildWest,
        SnowMap,
        FootBallMap,
        SampleScene
    }
    [SerializeField]private Button wildWestButton;
    [SerializeField]private Button snowMapButton;
    [SerializeField]private Button FootBallMapButton;
    [SerializeField]private Button SampleScene;

    [SerializeField]private Color selectedColor;
    [SerializeField]private Color resetColor;


    private NetworkVariable<MapName> mapName = new NetworkVariable<MapName>(MapName.WildWest); 
    private void Awake(){
        Instance = this;
        wildWestButton.GetComponent<Outline>().effectColor = selectedColor;

        // add online effect when clicked
        wildWestButton.onClick.AddListener(()=>{
            RemoveAllOutline();
            wildWestButton.GetComponent<Outline>().effectColor = selectedColor;
            mapName.Value = MapName.WildWest;
        });
        snowMapButton.onClick.AddListener(()=>{
            RemoveAllOutline();
            snowMapButton.GetComponent<Outline>().effectColor = selectedColor;
            mapName.Value = MapName.SnowMap;
        });
        FootBallMapButton.onClick.AddListener(()=>{
            RemoveAllOutline();
            FootBallMapButton.GetComponent<Outline>().effectColor = selectedColor;
            mapName.Value = MapName.FootBallMap;
        });
        SampleScene.onClick.AddListener(()=>{
            RemoveAllOutline();
            SampleScene.GetComponent<Outline>().effectColor = selectedColor;
            mapName.Value = MapName.SampleScene;
        });
    }
    private void RemoveAllOutline(){
        Button[] allButtons = {wildWestButton , snowMapButton , FootBallMapButton , SampleScene};
        foreach(Button button in allButtons){
            button.GetComponent<Outline>().effectColor = resetColor;
        }
    }
    public override void OnNetworkSpawn()
    {
        if(IsHost){
            gameObject.SetActive(true); 
        }else{
            gameObject.SetActive(false); 
        }
    }
    public MapName GetMapName(){
        return mapName.Value;
    }

}
