using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField]private AudioClip victoryAudioClip;
    [SerializeField]private AudioClip defeatAudioClip;

    void Awake()
    {
        Instance = this;
    }
    public void PlayDefeatSound(){
        AudioSource.PlayClipAtPoint(defeatAudioClip , transform.position);
    }
    public void PlayVictorySound(){
        AudioSource.PlayClipAtPoint(victoryAudioClip , transform.position);
    }
}
