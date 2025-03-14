using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHoldingBall : NetworkBehaviour
{
    [SerializeField]private LayerMask ballLayer;
    [SerializeField]private int overlapRadius;
    [SerializeField]private Transform holdingPosition;
    [SerializeField]private PlayerAnimation playerAnimation;
    [SerializeField]private PlayerCharacterVisual playerCharacterVisual;
    [SerializeField]private LayerMask opponentLayerMask ;
    [SerializeField]private int maxAngle;
    private bool holdingBall=false;
    public BaseThrowBall holdBall;
    private Collider[] opponentCollider = new Collider[2];
    private Vector3 ballLaunchDirection = Vector3.zero;
    private Transform opponentTransform;
    private void Start() {
        GameInput.Instnace.OnInteractBallPickUp += PickThrowBall;
        // AssignAnimationOnServerRpc();
    }
    // [ServerRpc(RequireOwnership =false)]
    // private void AssignAnimationOnServerRpc(){
    //     playerAnimation = playerCharacterVisual.transform.GetChild(0).GetComponent<PlayerAnimation>();
    // }

    public void DestroyBallOnServer(){
        Debug.Log(holdBall);
        if(holdBall!=null){
            DodgeGameMultiplayer.Instance.DestroyBallObject(holdBall);
        }
    }
    IEnumerator AimAtOpponent() {
        while (true) {
            int opponentColliderCount = Physics.OverlapSphereNonAlloc(transform.position, 25, opponentCollider, opponentLayerMask);

            for (int i = 0; i < opponentColliderCount; i++) {
                // if(opponentCollider[i] == null) continue; // IDK
                opponentTransform = opponentCollider[i].transform;
                Vector3 targetPosition = (opponentTransform.position - transform.position).normalized;
                float angle = Vector3.Angle(transform.forward , targetPosition);
                if (angle < maxAngle) { 
                    ballLaunchDirection = targetPosition;
                    opponentTransform.GetComponent<Player>().SetScopedMarker(true);
                    break;
                }
                opponentTransform.GetComponent<Player>().SetScopedMarker(false);
                ballLaunchDirection = transform.forward;
            }
            yield return new WaitForSeconds(0.1f); 
        }

    }

    private void PickThrowBall(object sender , System.EventArgs e){
        if(!IsOwner) return;
        if(!holdingBall){
            // Pick up the ball
            Collider[] collider = Physics.OverlapSphere(transform.position , overlapRadius ,ballLayer);
            if(collider.Length != 0){
                collider[0].gameObject.TryGetComponent(out BaseThrowBall ball);
                if (ball!=null && !ball.attackingBall.Value){
                    holdingBall = true;
                    ball.SetPlayerTransform(NetworkObject);
                    StartCoroutine(AimAtOpponent());
                    ChangeThrowPickIconUI.Instance.ToggleGrabIcon(false);
                    // holdBall = ball;
                }
            }
        }else{
            // Throw Ball
            StopAllCoroutines();
            if(opponentTransform.TryGetComponent<Player>(out Player player)){
                player.SetScopedMarker(false);
            }
            ChangeThrowPickIconUI.Instance.ToggleGrabIcon(true);
            playerAnimation.PlayThrowAnimation();
            holdingBall = false;
            if(ballLaunchDirection==Vector3.zero){
                ballLaunchDirection = transform.forward;
            }
            holdBall.LaunchBall(ballLaunchDirection , NetworkObject);
            // this.holdBall = null;
        }
    }
    private void FetchTargetTransform(){
        int opponentColliderCount = Physics.OverlapSphereNonAlloc(transform.position , 10 , opponentCollider ,opponentLayerMask);
        for(int i = 0;i<opponentColliderCount;i++){
            // check if player exists in map
            Vector3 targetDirection = (opponentCollider[i].gameObject.transform.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.forward , targetDirection);
            if(dot > 0.8f){
                // player facing the opponent
                ballLaunchDirection = targetDirection;
                break;
            }
        }
    }
    public Transform BallParentTransform(){
        return holdingPosition;
    }
    public bool IsHolding(){
        return holdingBall;
    }
    public BaseThrowBall GetHoldingBallObject(){
        return holdBall;
    }
    public bool HasHoldingBallObject(){
        return holdBall != null;
    }
    public void SetBaseThrowBall(BaseThrowBall holdBall){
        this.holdBall = holdBall;
    }
}
