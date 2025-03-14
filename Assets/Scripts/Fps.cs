using TMPro;
using UnityEngine;

public class Fps : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float nextTime;
    [SerializeField]private float delayTime = 0.1f;
    private void Start() {
        Application.targetFrameRate = 60;
        nextTime = Time.time;
    }
    void Update()
    {
        if(Time.time > nextTime){
            float fps = 1.0f / Time.unscaledDeltaTime;
            fpsText.SetText($"FPS: {Mathf.Round(fps)}");
            nextTime = Time.time + delayTime;
        }
    }
}
