using UnityEngine;

[CreateAssetMenu(fileName = "BaseThrowBallSO", menuName = "Scriptable Objects/BaseThrowBallSO")]
public class BaseThrowBallSO : ScriptableObject
{
    public int launchForce;
    public LayerMask playerLayer;

    public float checkInterval = 0.1f;
    public GameObject hitPlayerParticleEffect;
    public float radius;
}
