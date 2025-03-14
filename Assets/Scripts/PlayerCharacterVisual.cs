using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterVisual : MonoBehaviour
{
    // [SerializeField]private int defaultPlayerCharacterIndexID;
    // private void Start(){
    //     Destroy(transform.GetChild(0));
    //     Instantiate(DodgeGameMultiplayer.Instance.GetPlayerVisualTransformObject(defaultPlayerCharacterIndexID));
    // }
    [SerializeField]private GameObject scopedMarker;
    void Start(){
        if(scopedMarker!=null){
            scopedMarker.SetActive(false);
        }
    }

    public void SetPlayerCharacterVisual(int playerCharacterIndexID){
        if(transform.childCount != 0){
            DestroyAllChildren();
        }
        Instantiate(DodgeGameMultiplayer.Instance.GetPlayerVisualTransformObject(playerCharacterIndexID) , transform);
    }
    private void DestroyAllChildren(){
        for(int i = 0;i <transform.childCount;i++){
            Destroy(transform.GetChild(i).gameObject);
        }
    }
    public void ToggleScopedMarker(bool isEnable){
        scopedMarker.SetActive(isEnable);
    }
}
