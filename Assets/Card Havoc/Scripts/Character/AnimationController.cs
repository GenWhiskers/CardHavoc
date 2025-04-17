using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;

     // Local cache to avoid creating strings every frame
    private int hashMoveX = Animator.StringToHash("Speed");
    private int hashMoveZ = Animator.StringToHash("Speed");
    private int hashIsRunning = Animator.StringToHash("Speed");
    private int hashIsJumping = Animator.StringToHash("Jump");
    private int hashIsGrounded = Animator.StringToHash("Grounded");
    
    private void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    #region Local Player Updates

    // Called by local player
    public void UpdateLocalAnimations(float moveX, float moveZ, bool isRunning, bool isJumping, bool isGrounded)
    {
        animator.SetFloat(hashMoveX, moveX);
        animator.SetFloat("MotionSpeed", moveZ);
        // animator.SetBool(hashIsRunning, isRunning);
        animator.SetBool(hashIsJumping, isJumping);
        animator.SetBool(hashIsGrounded, isGrounded);
    }

    // Called by other players via RPC
    public void UpdateRemoteAnimations(float moveX, float moveZ, bool isRunning, bool isJumping, bool isGrounded)
    {
        // Same implementation, but called via network
        animator.SetFloat(hashMoveX, moveX);
        animator.SetFloat("MotionSpeed", moveZ);
        // animator.SetBool(hashIsRunning, isRunning);
        animator.SetBool(hashIsJumping, isJumping);
        animator.SetBool(hashIsGrounded, isGrounded);
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