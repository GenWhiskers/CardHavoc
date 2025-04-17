using UnityEngine;
using FishNet.Object;

public class PlayerAnimationController : NetworkBehaviour
{
    [Header("Animator Setup")]
    [SerializeField] private Animator animator;

    // Animator parameter hashes
    private int speedHash;
    private int groundedHash;
    private int jumpHash;
    private int freeFallHash;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        speedHash = Animator.StringToHash("Speed");
        groundedHash = Animator.StringToHash("Grounded");
        jumpHash = Animator.StringToHash("Jump");
        freeFallHash = Animator.StringToHash("FreeFall");
    }

    #region Local Player Updates

    /// <summary>
    /// Called by PlayerController to update local animation state.
    /// </summary>
    public void UpdateLocalAnimations(float speed, float inputMagnitude, bool grounded, bool jumping, bool falling)
    {
        if (!IsOwner) return;

        animator.SetFloat(speedHash, speed);
        animator.SetFloat("MotionSpeed", inputMagnitude);
        animator.SetBool(groundedHash, grounded);
        animator.SetBool(jumpHash, jumping);
        animator.SetBool(freeFallHash, falling);

        // Also sync to remote clients
        UpdateRemoteAnimations(speed, inputMagnitude, grounded, jumping, falling);
    }

    #endregion

    #region Remote Animation Sync

    [ObserversRpc]
    private void UpdateRemoteAnimations(float speed, float inputMagnitude, bool grounded, bool jumping, bool falling)
    {
        if (IsOwner) return; // Owner already set their own animator

        if (animator == null)
        {
            Debug.LogWarning("Remote animator is null!");
            return;
        }
        animator.SetFloat(speedHash, speed);
        animator.SetFloat("MotionSpeed", inputMagnitude);
        animator.SetBool(groundedHash, grounded);
        animator.SetBool(jumpHash, jumping);
        animator.SetBool(freeFallHash, falling);
    }

    #endregion

    #region Animation Events

    public void OnFootstep(AnimationEvent evt)
    {
        // Hook up footstep SFX here
    }

    public void OnLand(AnimationEvent evt)
    {
        // Hook up landing SFX here
    }

    #endregion
}