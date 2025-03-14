using UnityEngine;

public class BallSoundManager : MonoBehaviour
{
    public static BallSoundManager Instance;
    [SerializeField]private AudioClip ballThrow;
    [SerializeField]private AudioClip ballHit;
    void Awake()
    {
        Instance = this;
    }

    public void PlayBallThrow(){
        AudioSource.PlayClipAtPoint(ballThrow , transform.position);
    }
    public void PlayBallHit(){
        AudioSource.PlayClipAtPoint(ballHit , transform.position);
    }
}
