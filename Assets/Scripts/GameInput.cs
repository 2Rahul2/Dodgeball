using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instnace {get;private set;}
    [SerializeField]private PlayerInput playerInput;
    public event EventHandler OnDashPerformed;
    public event EventHandler OnInteractBallPickUp;
    public event EventHandler OnMobileDashCancelled;
    public event EventHandler OnMobileDashPerformed;
    private Vector2 DashCoordinates = Vector3.zero;

    private void Awake() {
        Instnace = this;
        playerInput.actions["Dash"].performed += DashPerformed;
        playerInput.actions["PickBall"].performed += PickBallPerformed;
        playerInput.actions["MoveDash"].canceled += MobileDashCancelled;
        playerInput.actions["MoveDash"].performed += MobileDashPerformed;
    }
    private void MobileDashCancelled(UnityEngine.InputSystem.InputAction.CallbackContext context){
        OnMobileDashCancelled?.Invoke(this , System.EventArgs.Empty);
    }
    private void MobileDashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context){
        DashCoordinates = context.ReadValue<Vector2>().normalized;
        OnMobileDashPerformed?.Invoke(this , System.EventArgs.Empty);
    }
    private void PickBallPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context){
        OnInteractBallPickUp?.Invoke(this , System.EventArgs.Empty);
    }
    private void DashPerformed(UnityEngine.InputSystem.InputAction.CallbackContext context){
       OnDashPerformed?.Invoke(this , System.EventArgs.Empty); 
    }
    public Vector2 GetMovementVectorNormalized(){
        return playerInput.actions["Move"].ReadValue<Vector2>().normalized;
    }
    public Vector2 GetMobileDashVectorNormalized(){
        return DashCoordinates;
    }

    public Vector2 GetDashDirectionVectorNormalized(){
        return playerInput.actions["MoveDash"].ReadValue<Vector2>().normalized;
    }

}
