using Unity.Netcode;
using UnityEngine;

public class PlayerAnimation : NetworkBehaviour
{
    private const string Move = "Move";
    private const string Idle = "Idle";

    public Animator animator;//come back and change it to private


    public void SetIsMoving(bool moving)
    {
        if (!IsOwner) return;

        // Update the local animator
        animator.SetBool(Move, moving);

        // Synchronize the animation state across the network
        SetIsMovingServerRpc(moving);
    }

    public void SetIsIdle(bool idle)
    {
        if (!IsOwner) return;

        // Update the local animator
        animator.SetBool(Idle, idle);

        // Synchronize the animation state across the network
        SetIsIdleServerRpc(idle);
    }

    public void PlayDashAnimation()
    {
        if (!IsOwner) return;

        // Trigger the animation locally
        animator.SetTrigger("Dash");

        // Synchronize the animation trigger across the network
        PlayDashAnimationServerRpc();
    }

    public void PlayThrowAnimation()
    {
        if (!IsOwner) return;

        // Trigger the animation locally
        animator.SetTrigger("Throw");

        // Synchronize the animation trigger across the network
        PlayThrowAnimationServerRpc();
    }

    public void SetAnimator(Animator animator)
    {
        this.animator = animator;
    }

    [ServerRpc]
    private void SetIsMovingServerRpc(bool moving)
    {
        // Update the animator on the server
        animator.SetBool(Move, moving);

        // Synchronize the animation state with all clients
        SetIsMovingClientRpc(moving);
    }

    [ClientRpc]
    private void SetIsMovingClientRpc(bool moving)
    {
        // Update the animator on all clients
        if (!IsOwner) // Avoid overriding the owner's animator
        {
            animator.SetBool(Move, moving);
        }
    }

    [ServerRpc]
    private void SetIsIdleServerRpc(bool idle)
    {
        // Update the animator on the server
        animator.SetBool(Idle, idle);

        // Synchronize the animation state with all clients
        SetIsIdleClientRpc(idle);
    }

    [ClientRpc]
    private void SetIsIdleClientRpc(bool idle)
    {
        // Update the animator on all clients
        if (!IsOwner) // Avoid overriding the owner's animator
        {
            animator.SetBool(Idle, idle);
        }
    }

    [ServerRpc]
    private void PlayDashAnimationServerRpc()
    {
        // Trigger the animation on the server
        animator.SetTrigger("Dash");

        // Synchronize the animation trigger with all clients
        PlayDashAnimationClientRpc();
    }

    [ClientRpc]
    private void PlayDashAnimationClientRpc()
    {
        // Trigger the animation on all clients
        if (!IsOwner) // Avoid overriding the owner's animator
        {
            animator.SetTrigger("Dash");
        }
    }

    [ServerRpc]
    private void PlayThrowAnimationServerRpc()
    {
        // Trigger the animation on the server
        animator.SetTrigger("Throw");

        // Synchronize the animation trigger with all clients
        PlayThrowAnimationClientRpc();
    }

    [ClientRpc]
    private void PlayThrowAnimationClientRpc()
    {
        // Trigger the animation on all clients
        if (!IsOwner) // Avoid overriding the owner's animator
        {
            animator.SetTrigger("Throw");
        }
    }
}
