using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected Animator animator;
    [SerializeField] private float groundCheckDistance = 0.3f;
    [SerializeField] private float ceilingCheckDistance = 0.3f;
    [SerializeField] private LayerMask groundMask;

    public string CurrentAnimation { get; private set; }
    public bool IsOnFloor { get; private set; }
    public bool IsOnCeiling { get; private set; }

    private RaycastHit floorHit;
    private RaycastHit ceilingHit;

    public bool canInput = true;
    protected Vector3 velocity = Vector3.zero;

    protected float originalHeight = 10f;

    public void PerformCheckers()
    {
        float radius = controller.radius;
        float halfHeight = controller.height / 2f;

        Vector3 bottom = transform.position + Vector3.down * (halfHeight - radius);
        Vector3 top = transform.position + Vector3.up * (halfHeight - radius);

        // Ground check
        IsOnFloor =
            Physics.SphereCast(bottom, radius, Vector3.down, out floorHit, groundCheckDistance, groundMask) ||
            Physics.Raycast(bottom, Vector3.down, out floorHit, groundCheckDistance, groundMask);

        // Ceiling check
        IsOnCeiling =
            Physics.SphereCast(top, radius, Vector3.up, out ceilingHit, ceilingCheckDistance, groundMask) ||
            Physics.Raycast(top, Vector3.up, out ceilingHit, ceilingCheckDistance, groundMask);
    }

    public void Move()
    {
        if (!canInput) return;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SetVelocity(float x, float y, float z)
    {
        velocity = new Vector3(x, y, z);
    }

    public void ApplyVelocity(Vector3 dir, float speed, float amount)
    {
        if (!canInput) return;

        velocity = Vector3.Lerp(velocity, new Vector3(dir.x * speed, velocity.y, dir.z * speed), amount * Time.deltaTime);
    }

    public void ApplyGravity()
    {
        velocity.y += Physics.gravity.y * Time.deltaTime;
    }

    public void PlayAnimation(string animationName, float transitionDuration = 0)
    {
        if (animationName != CurrentAnimation)
        {
            CurrentAnimation = animationName;
            animator.CrossFade(animationName, transitionDuration);
        }
    }

    public bool IsAnimationPlaying(string animationName)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(animationName);
    }

    #region Debugging
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

        // Draw ground check
        drawer.DrawRaySphereCheck(bottom, Vector3.down, groundCheckDistance, radius, Color.green);

        // Draw ceiling check
        drawer.DrawRaySphereCheck(top, Vector3.up, ceilingCheckDistance, radius, Color.cyan);
    }
    #endregion


}
