using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected CharacterController controller; // Reference to CharacterController component
    [SerializeField] protected Animator animator;             // Reference to Animator component
    [SerializeField] private float groundCheckDistance = 0.3f; // Distance for ground checking
    [SerializeField] private float ceilingCheckDistance = 0.3f;// Distance for ceiling checking
    [SerializeField] private LayerMask groundMask;             // Layers considered as ground

    public string CurrentAnimation { get; private set; }       // Currently playing animation name
    public bool IsOnFloor { get; private set; }                // Flag for ground contact
    public bool IsOnCeiling { get; private set; }              // Flag for ceiling contact

    private RaycastHit floorHit;                                // Info about ground hit
    private RaycastHit ceilingHit;                              // Info about ceiling hit

    public bool canInput = true;                                // Whether input is allowed
    protected Vector3 velocity = Vector3.zero;                  // Current movement velocity

    protected float originalHeight = 10f;                       // Original controller height (for possible resizing)

    // Performs ground and ceiling checks using sphere casts and raycasts
    public void PerformCheckers()
    {
        float radius = controller.radius;
        float halfHeight = controller.height / 2f;

        Vector3 bottom = transform.position + Vector3.down * (halfHeight - radius);
        Vector3 top = transform.position + Vector3.up * (halfHeight - radius);

        IsOnFloor =
            Physics.SphereCast(bottom, radius, Vector3.down, out floorHit, groundCheckDistance, groundMask) ||
            Physics.Raycast(bottom, Vector3.down, out floorHit, groundCheckDistance, groundMask);

        IsOnCeiling =
            Physics.SphereCast(top, radius, Vector3.up, out ceilingHit, ceilingCheckDistance, groundMask) ||
            Physics.Raycast(top, Vector3.up, out ceilingHit, ceilingCheckDistance, groundMask);
    }

    // Moves the character using velocity if input allowed
    public void Move()
    {
        if (!canInput) return;

        controller.Move(velocity * Time.deltaTime);
    }

    // Sets the velocity vector directly
    public void SetVelocity(float x, float y, float z)
    {
        velocity = new Vector3(x, y, z);
    }

    // Applies velocity towards a target horizontal direction and speed, smoothing the transition
    public void ApplyVelocity(Vector3 dir, float speed, float amount)
    {
        if (!canInput) return;

        velocity = Vector3.Lerp(velocity, new Vector3(dir.x * speed, velocity.y, dir.z * speed), amount * Time.deltaTime);
    }

    // Applies gravity to vertical velocity
    public void ApplyGravity()
    {
        velocity.y += Physics.gravity.y * Time.deltaTime;
    }

    // Plays animation using crossfade; only triggers if different from current
    public void PlayAnimation(string animationName, float transitionDuration = 0)
    {
        if (animationName != CurrentAnimation)
        {
            CurrentAnimation = animationName;
            animator.CrossFade(animationName, transitionDuration);
        }
    }

    // Checks if specified animation is currently playing
    public bool IsAnimationPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    #region Debugging
    // Draw gizmos for ground and ceiling checks when object is selected in editor
    private void OnDrawGizmosSelected()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();

        if (controller == null)
            return;

        DebugDrawer drawer = new DebugDrawer();

        float radius = controller.radius;
        float halfHeight = controller.height / 2f;
        Vector3 bottom = transform.position + Vector3.down * (halfHeight - radius);
        Vector3 top = transform.position + Vector3.up * (halfHeight - radius);

        drawer.DrawRaySphereCheck(bottom, Vector3.down, groundCheckDistance, radius, Color.green);
        drawer.DrawRaySphereCheck(top, Vector3.up, ceilingCheckDistance, radius, Color.cyan);
    }
    #endregion
}
