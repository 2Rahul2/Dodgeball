using UnityEngine;

public class TeamHighLightVisual : MonoBehaviour
{
    public static TeamHighLightVisual Instance {get;private set;}
    [SerializeField]private Transform blueMarkerVisualObject;
    [SerializeField]private Transform redMarkerVisualObject;
    [Header("Player Visuals")]
    [SerializeField]private GameObject playerBlueMarker;
    [SerializeField]private GameObject playerRedMarker;
    private void Awake(){
        Instance = this;
    }
    public void BlueMarkTeamMatesVisual(Transform player){
        GameObject particleObject = Instantiate(playerBlueMarker ,player.position , Quaternion.identity);
        particleObject.transform.SetParent(player);
        TransformMarkerShapes(particleObject);
    }
    public void RedMarkTeamMatesVisual(Transform player){
        GameObject particleObject = Instantiate(playerRedMarker ,player.position , Quaternion.identity);
        particleObject.transform.SetParent(player);
        TransformMarkerShapes(particleObject);

        // Instantiate(playerRedMarker ,player);
    }
    private void TransformMarkerShapes(GameObject marker){
        marker.transform.position += new Vector3(0f,0.2f,0f);
        marker.transform.rotation = Quaternion.Euler(new Vector3(-90 ,0 ,0));
    }
}
