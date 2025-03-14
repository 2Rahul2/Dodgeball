using Unity.Netcode;
using UnityEngine;

public class ThreeBallsAttack : BaseThrowBall
{
    [SerializeField]private GameObject basicThrowBall; 
    public override void LaunchBall(Vector3 throwDirection ,NetworkObjectReference networkObjectReference){
        base.LaunchBall(throwDirection, networkObjectReference);

        LaunchThreeBallServerRpc(throwDirection , networkObjectReference);
    }

    [ServerRpc(RequireOwnership =false)]
    public void LaunchThreeBallServerRpc(Vector3 throwDirection ,NetworkObjectReference playerHoldingBallReference){
        GameObject ballOne = Instantiate(basicThrowBall , transform.position + (transform.right * 1f) ,Quaternion.identity);
        GameObject ballTwo = Instantiate(basicThrowBall , transform.position + (transform.right * -1f) ,Quaternion.identity);
        NetworkObject throwBallNetworkObjectOne = ballOne.GetComponent<NetworkObject>();
        throwBallNetworkObjectOne.Spawn(true);
        NetworkObject throwBallNetworkObjectTwo = ballTwo.GetComponent<NetworkObject>();
        throwBallNetworkObjectTwo.Spawn(true);

        BaseThrowBall baseThrowBallOne = ballOne.GetComponent<BaseThrowBall>();
        BaseThrowBall baseThrowBallTwo = ballTwo.GetComponent<BaseThrowBall>();

        baseThrowBallOne.attackingBall.Value = true;
        baseThrowBallTwo.attackingBall.Value = true;


        baseThrowBallOne.parentTeamId.Value = parentTeamId.Value;
        baseThrowBallTwo.parentTeamId.Value = parentTeamId.Value;

        baseThrowBallOne.LaunchBall(throwDirection+(transform.right*0.1f),playerHoldingBallReference);
        baseThrowBallTwo.LaunchBall(throwDirection+(transform.right* -0.1f),playerHoldingBallReference);
    }

}
