using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class BaseThrowBall : NetworkRigidbody
{
    [SerializeField]protected BaseThrowBallSO ballObjectSO;
    [SerializeField]protected Rigidbody rb;
    // [SerializeField]protected int launchForce;
    // [SerializeField]private float radius;
    // [SerializeField]private LayerMask playerLayer;
    protected Collider[] detectedPlayers = new Collider[4];

    private float nextCheckTime;
    [HideInInspector]public NetworkVariable<bool> attackingBall=new(false);
    [SerializeField]private BallVisual ballVisual;
    // [SerializeField]private GameObject hitPlayerParticleEffect;
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }
    protected Transform playerTransform;
    public NetworkVariable<int> parentTeamId = new();
    public NetworkVariable<float> radius = new();
    [SerializeField]protected NetworkVariable<float> initialRadius;
    public override void OnNetworkSpawn(){
        SetRadiusServerRpc();
    }
    [ServerRpc(RequireOwnership =false)]
    private void SetRadiusServerRpc(){
        radius.Value = GetComponent<SphereCollider>().radius + 0.4f;
        initialRadius.Value = radius.Value;
    }
    public override void OnDestroy(){
        StopAllCoroutines();
    }
    private void FixedUpdate() {
        if(attackingBall.Value){
            DetectPlayerContact();
            // if(Time.time > nextCheckTime){
                // nextCheckTime = Time.time + ballObjectSO.checkInterval;
            // }
        }
    }
    private void Update(){
        if (playerTransform == null)return;

        transform.SetPositionAndRotation(playerTransform.position , playerTransform.rotation);    
    }
    protected void DetectPlayerContact(){
        int detectedCount = Physics.OverlapSphereNonAlloc(transform.position , radius.Value , detectedPlayers ,ballObjectSO.playerLayer);
        for(int i=0;i<detectedCount ;i++){
            if(parentTeamId.Value != detectedPlayers[i].GetComponent<Player>().GetTeamId()){
                if(!detectedPlayers[i].GetComponent<Player>().GetGotHit()){
                    detectedPlayers[i].GetComponent<Player>().SetGotHitServerRpc(true);
                    HurtPlayerServerRpc(parentTeamId.Value , detectedPlayers[i].GetComponent<NetworkObject>().OwnerClientId);
                }
            }     
        }
    }
    [ServerRpc(RequireOwnership =false)]
    protected void HurtPlayerServerRpc(int teamid,ulong opponentClientId){
        ScoreNetwork.Instance.DecidePlayerWinServerRpc(teamid ,opponentClientId);
        HurtPlayerClientRpc();
    }
    [ClientRpc]
    private void HurtPlayerClientRpc(){
        // hurt the player
        Instantiate(ballObjectSO.hitPlayerParticleEffect , transform.position , quaternion.identity);
        // Destroy(gameObject); 
        StopAllCoroutines();
        DestroyBallObject(this);
    }
    public void SetPlayerTransform(NetworkObjectReference parentTransformNetworkObject){
        SetPlayerTransformServerRpc(parentTransformNetworkObject);
    }
    public virtual void LaunchBall(Vector3 throwDirection ,NetworkObjectReference networkObjectReference){
        LaunchBallServerRpc(throwDirection ,networkObjectReference);
    }

    [ServerRpc(RequireOwnership =false)]
    private void SetPlayerTransformServerRpc(NetworkObjectReference parentTransformNetworkObject){
        rb.useGravity = false;
        rb.isKinematic = true;
        
        parentTransformNetworkObject.TryGet(out NetworkObject playerHoldingBallNetworkObject);
        PlayerHoldingBall playerHoldingBall = playerHoldingBallNetworkObject.GetComponent<PlayerHoldingBall>();
        // transform.SetParent(playerHoldingBall.BallParentTransform());
        parentTeamId.Value = playerHoldingBall.GetComponent<Player>().GetTeamId();
        playerTransform = playerHoldingBall.BallParentTransform();
        SetParentHoldBallClientRpc(parentTransformNetworkObject);
        // transform.SetParent(parentTransform);
    }
    [ClientRpc]
    private void SetParentHoldBallClientRpc(NetworkObjectReference playerHoldingBallReference){
        playerHoldingBallReference.TryGet(out NetworkObject playerHoldingBallNetWorkObject);
        PlayerHoldingBall playerHoldingBall = playerHoldingBallNetWorkObject.GetComponent<PlayerHoldingBall>();
        if(playerHoldingBall != null){
            playerHoldingBall.SetBaseThrowBall(this);
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void LaunchBallServerRpc(Vector3 throwDirection ,NetworkObjectReference playerHoldingBallReference){
        playerTransform = null;
        attackingBall.Value = true;
        rb.useGravity = true;
        rb.isKinematic = false;
        rb.AddForce(throwDirection * ballObjectSO.launchForce , ForceMode.Impulse);
        // BallSpawnManager.Instance.RemoveCurrentSpawnBalls();
        SetParentNullHoldBallClientRpc(playerHoldingBallReference);
    }
    [ClientRpc]
    protected void SetParentNullHoldBallClientRpc(NetworkObjectReference playerHoldingBallReference){
        // Destroy(gameObject , 5f);
        ballVisual.StartVisualEffects();
        BallSoundManager.Instance.PlayBallThrow();
        playerHoldingBallReference.TryGet(out NetworkObject playerHoldingBallNetWorkObject);
        PlayerHoldingBall playerHoldingBall = playerHoldingBallNetWorkObject.GetComponent<PlayerHoldingBall>();
        if(playerHoldingBall != null){
            playerHoldingBall.SetBaseThrowBall(null);
            BallSpawnManager.Instance.RemoveCurrentSpawnBalls();
        }
        StartCoroutine(SelfDestructWithTimer());
    }
    IEnumerator SelfDestructWithTimer(){
        yield return new WaitForSeconds(3f);
        DestroyBallObject(this);
    } 
    public void DestroySelf(){
        Destroy(gameObject);
    }

    public static void DestroyBallObject(BaseThrowBall baseThrowBall){
        DodgeGameMultiplayer.Instance.DestroyBallObject(baseThrowBall);
    }

}
