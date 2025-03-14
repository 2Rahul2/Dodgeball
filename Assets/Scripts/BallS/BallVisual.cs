using System.Collections;
using UnityEngine;

public class BallVisual : MonoBehaviour
{
    [SerializeField]private BaseThrowBall baseThrowBall;
    [SerializeField]private ParticleSystem flareEffect;
    [SerializeField]private Transform[] trailEffectList;
    public void StartVisualEffects(){
        flareEffect.Play();
        StartCoroutine(DyanmicFlareSlowDownEffect());
        foreach(Transform trail in trailEffectList){
            trail.gameObject.SetActive(true);
        }
    }
    IEnumerator DyanmicFlareSlowDownEffect(){
        while(true){
            flareEffect.transform.rotation = baseThrowBall.transform.rotation;
            if (baseThrowBall.GetLinearVelocity().magnitude < 2f){
               flareEffect.Stop(); 
            }else{
               flareEffect.Play(); 
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
