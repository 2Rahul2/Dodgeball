using UnityEngine;
using UnityEngine.UI;

public class ChangeThrowPickIconUI : MonoBehaviour
{
    [SerializeField]private Sprite grabIcon;
    [SerializeField]private Sprite throwIcon;

    [SerializeField]private RectTransform throwPickUpIcon;
    public static ChangeThrowPickIconUI Instance;
    void Awake(){
        Instance = this;
    }
    public void ToggleGrabIcon(bool isGrab){
        if(isGrab){
            throwPickUpIcon.GetComponent<Image>().sprite = grabIcon;
        }else{
            throwPickUpIcon.GetComponent<Image>().sprite = throwIcon;
        }
    }
}
