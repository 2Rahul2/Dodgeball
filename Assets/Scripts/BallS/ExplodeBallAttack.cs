using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ExplodeBallAttack : BaseThrowBall
{
    [SerializeField]private GameObject explosionEffect;
    [SerializeField]private float explodeTime=1f;
    IEnumerator StartCountDown(){
        yield return new WaitForSeconds(explodeTime);
        ExplodeBallServerRpc();
    }

    public override void LaunchBall(Vector3 throwDirection, NetworkObjectReference networkObjectReference)
    {
        base.LaunchBall(throwDirection, networkObjectReference);
        StartCoroutine(StartCountDown());
    }
    [ServerRpc(RequireOwnership =false)]
    private void ExplodeBallServerRpc(){
        ExplodeBallClientRpc();
    }
    [ClientRpc]
    private void ExplodeBallClientRpc(){
        Instantiate(explosionEffect , transform.position, transform.rotation);
        // damage enemies
        int detectedCount = Physics.OverlapSphereNonAlloc(transform.position , ballObjectSO.radius+1 , detectedPlayers ,ballObjectSO.playerLayer);
        for(int i=0;i<detectedCount ;i++){
            if(parentTeamId.Value != detectedPlayers[i].GetComponent<Player>().GetTeamId()){
                Debug.Log("Exploded Player");
                HurtPlayerServerRpc(parentTeamId.Value ,detectedPlayers[i].GetComponent<NetworkObject>().OwnerClientId);
            }     
        }
    }

}
