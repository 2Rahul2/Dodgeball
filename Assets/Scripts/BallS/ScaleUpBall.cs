using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ScaleUpBall : BaseThrowBall
{
    private Vector3 finalBallSize = new Vector3(6 , 6, 6);
    [SerializeField]private int scaleDuration;
    private float elapsedTime = 0f;
    Vector3 initialSize;
    public override void LaunchBall(Vector3 throwDirection, NetworkObjectReference networkObjectReference)
    {
        base.LaunchBall(throwDirection, networkObjectReference);
        initialSize = transform.localScale;
        StartCoroutine(UpdateScaleAttack());
    }
    IEnumerator UpdateScaleAttack(){
        while(true){
            if(elapsedTime < scaleDuration){
                elapsedTime += Time.deltaTime;
                transform.localScale = Vector3.Lerp(initialSize , finalBallSize , elapsedTime/scaleDuration);
                float newRadius = initialRadius.Value * (transform.localScale.x / initialSize.x);
                ChangeRadiusServerRpc(newRadius);
            }
            yield return null;
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void ChangeRadiusServerRpc(float newRadius){
        radius.Value = newRadius;
    }
}
