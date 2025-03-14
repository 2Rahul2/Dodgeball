using System.Collections;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using System;


public class Player : NetworkBehaviour {
    public static Player LocalInstance {get ; private set;}
    [SerializeField]private LayerMask collisionsLayerMask;
    // private Rigidbody rb;
    [Header("Player Movements")]
    [SerializeField]private int moveSpeed , moveMultiplier;
    [SerializeField]private int rotateSpeed;

    [Header("Dash Controls")]
    [SerializeField]private Transform dashDirectionParticle;
    [SerializeField]private int dashForce;
    [SerializeField]private float dashDuration;
    // private float dashStartTime;
    [SerializeField]private int maxDashSpeed;
    public bool canDash = true;
    private bool isDashing=false;
    private Vector3 dashDirection;
    [SerializeField]private ParticleSystem dashParticleSystem;

    


    [SerializeField]private PlayerAnimation playerAnimation;
    private PlayerHoldingBall playerHoldingBall;
    [SerializeField]private PlayerCharacterVisual playerCharacterVisual;
    public int teamID;
    private void Start() {
        dashDirectionParticle.gameObject.SetActive(false);
        playerHoldingBall = GetComponent<PlayerHoldingBall>();
        GameInput.Instnace.OnMobileDashCancelled += OnDashCancelledClient;
        GameInput.Instnace.OnMobileDashPerformed += OnDashPerformedClient;

        
        ScoreNetwork.Instance.OnPlayerGotHit_DisablePlayer += ScoreNetwork_OnPlayerGotHit_DisablePlayer;
        // transform.localScale = new Vector3(1.5f ,1.5f ,1.5f);
        if(teamID == 1){
            transform.position = DodgeGameManager.Instance.teamTwoPosition.position;
        }else{
            transform.position = DodgeGameManager.Instance.teamOnePosition.position;
        }

    }

    private void ScoreNetwork_OnPlayerGotHit_DisablePlayer(object sender, ScoreNetwork.OnPlayerGotHit_PlayerId e)
    {
        DisablePlayer( e.isEnable ,e.clientId);
    }

    [ServerRpc(RequireOwnership =false)]
    private void AssignAnimationOnServerRpc(){
        playerAnimation.SetAnimator(playerCharacterVisual.transform.GetChild(0).GetComponent<Animator>());
        playerCharacterVisual.transform.GetChild(0).transform.localScale = new Vector3(1.5f ,1.5f , 1.5f);
        playerCharacterVisual.transform.GetChild(0).transform.position = Vector3.zero;
    }
    public NetworkVariable<bool> gotHit = new NetworkVariable<bool>(false);


    // BallSpawnManager.Instance.StartSpawningRoutineServerRpc();
    public override void OnNetworkSpawn()
    {
        if(IsOwner){
            LocalInstance = this;
            // dashDirectionParticle.gameObject.SetActive(true);

        }
        if(IsServer){
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            // AssignAnimationOnServerRpc();
        }
        

        PlayerData playerData = DodgeGameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
        playerCharacterVisual.SetPlayerCharacterVisual(playerData.playerCharacterVisualIndex);
        playerAnimation.SetAnimator(playerCharacterVisual.transform.GetChild(0).GetComponent<Animator>());
        playerCharacterVisual.transform.GetChild(0).transform.localScale = new Vector3(1.5f ,1.5f , 1.5f);
        playerCharacterVisual.transform.GetChild(0).transform.position = Vector3.zero;
        // GetComponent<NetworkAnimator>().Animator = playerCharacterVisual.transform.GetChild(0).GetComponent<Animator>();

        
    }
    private void NetworkManager_OnClientDisconnectCallback(ulong clientId){
        Debug.Log("disconnect event triggered");
        if(clientId == OwnerClientId){
            Debug.Log("client id matched"+gameObject.name);
            if(playerHoldingBall.HasHoldingBallObject()){
                Debug.Log("Player holding a ball!");
                Debug.Log(playerHoldingBall.GetHoldingBallObject().NetworkObject.IsSpawned);
                BaseThrowBall.DestroyBallObject(playerHoldingBall.GetHoldingBallObject());

            }
            // DestroyBallOnServer();
        }
    }

    private void Update() {
        if(!IsOwner)return;

        if(!isDashing){
            HandleMovement();
            HandleDashDirectionParticle();
            // MovePlayerServerAuth();
        }
    }
    private void HandleDashDirectionParticle(){
        Vector2 rotateDir = GameInput.Instnace.GetDashDirectionVectorNormalized();
        if(rotateDir == Vector2.zero)return;
        Vector3 rotatePos = new Vector3(rotateDir.x , 0 ,rotateDir.y);

        dashDirectionParticle.forward = Vector3.Slerp(dashDirectionParticle.forward , rotatePos ,rotateSpeed * Time.deltaTime);
    }
    private void HandleMovement(){
        Vector2 moveInput = GameInput.Instnace.GetMovementVectorNormalized();
        Vector3 movePos = new Vector3(moveInput.x , 0  , moveInput.y);
        float moveDistance =  moveSpeed * moveMultiplier * Time.deltaTime;
        bool canMove = !Physics.BoxCast(transform.position, Vector3.one * 1.6f, movePos, Quaternion.identity, moveDistance, collisionsLayerMask);
        if(!canMove)return;
        if (moveInput == Vector2.zero){
            playerAnimation.SetIsMoving(false);
            playerAnimation.SetIsIdle(true);
            return;
        }
        playerAnimation.SetIsMoving(true);
        playerAnimation.SetIsIdle(false);

        transform.position += movePos * moveDistance;
        transform.forward = Vector3.Slerp(transform.forward , movePos,rotateSpeed * Time.deltaTime);

    }
    // private void MovePlayerServerAuth(){
    //     if (!IsOwner) return;
    //     Vector2 moveInput = GameInput.Instnace.GetMovementVectorNormalized();
    //     MovePlayerServerRpc(moveInput);
    // }
    // [ServerRpc(RequireOwnership =false)]
    // private void MovePlayerServerRpc(Vector2 moveInput){
    //     if (moveInput == Vector2.zero){
    //         playerAnimation.SetIsMoving(false);
    //         playerAnimation.SetIsIdle(true);
    //         return;
    //     }
    //     playerAnimation.SetIsMoving(true);
    //     playerAnimation.SetIsIdle(false);

    //     Vector3 movePos = new Vector3(moveInput.x , 0  , moveInput.y);
    //     transform.position += movePos * moveSpeed * moveMultiplier * Time.deltaTime;
    //     transform.forward = Vector3.Slerp(transform.forward , movePos,rotateSpeed * Time.deltaTime);
    // }
    private void OnDashCancelledClient(object sender , System.EventArgs e){
        if(IsOwner){
            dashDirectionParticle.gameObject.SetActive(false);
        }
        if(canDash && !playerHoldingBall.IsHolding()){
            canDash = false;
            playerAnimation.PlayDashAnimation();
            Vector2 moveInput = GameInput.Instnace.GetMobileDashVectorNormalized();
            if(!isDashing){
                StartCoroutine(DashNormal(moveInput));
            }
        }
    }
    private void OnDashPerformedClient(object sender , System.EventArgs e){
        if(IsOwner)
        dashDirectionParticle.gameObject.SetActive(true);
    }

    // private void OnDashCancelledServerAuth(object sender , System.EventArgs e){
    //     if(!IsOwner)return;

    //     if(canDash && !playerHoldingBall.IsHolding()){
    //         canDash = false;
    //         playerAnimation.PlayDashAnimation();
    //         Vector2 moveInput = GameInput.Instnace.GetMobileDashVectorNormalized();
    //         DashNormalServerRpc(moveInput);
    //     }
    // }
    // [ServerRpc(RequireOwnership =false)]
    // private void DashNormalServerRpc(Vector2 moveInput){
    //     if(!isDashing){
    //         StartCoroutine(DashNormal(moveInput));
    //     }
    // }
    IEnumerator DashNormal(Vector2 moveInput){
        if(IsOwner){
            dashParticleSystem.Play();
        }
        isDashing = true;
        dashDirection = new Vector3(moveInput.x, 0, moveInput.y);
        transform.LookAt(transform.position + dashDirection);
        float dashTimer = 0;
        while(dashTimer < dashDuration){
            float moveDistance = maxDashSpeed * Time.deltaTime;
            bool canMove = !Physics.BoxCast(transform.position, Vector3.one * 0.6f, dashDirection, Quaternion.identity, moveDistance, collisionsLayerMask);
            if(!canMove)yield return null;
            transform.position += dashDirection *moveDistance ;
            dashTimer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        canDash = true;
    }
    public void SetTeamId(int id){
        teamID = id;
    }
    public int GetTeamId(){
        return teamID;
    }
    public bool GetGotHit(){
        return gotHit.Value;
    }
    [ServerRpc(RequireOwnership =false)]
    public void SetGotHitServerRpc(bool isHit){
        gotHit.Value = isHit;
    }
    
    private void DisablePlayer(bool isEnable, ulong clientId){
        if(clientId == OwnerClientId){
            gameObject.SetActive(isEnable);
            if(isEnable){
                SetGotHitServerRpc(false);
            }else{
                StopAllCoroutines();
            }
        }
    }
    public void SetScopedMarker(bool isEnable){
        playerCharacterVisual.ToggleScopedMarker(isEnable);
    }
    // IEnumerator ResetDash(){
    //     isDashing = true;
    //     yield return new WaitForSeconds(dashDuration);
    //     isDashing = false;
    //     yield return new WaitForSeconds(0.6f);
    //     canDash = true;
    // }    
} 


