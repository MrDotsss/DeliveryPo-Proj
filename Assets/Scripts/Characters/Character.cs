using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [Header("Base")]
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected Animator animator;
    [SerializeField] protected float baseHeight = 2;
    [SerializeField] private float baseThickness = 0.8f;
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

    public void PerformCheckers()
    {
        IsOnFloor = Physics.SphereCast(transform.position, baseThickness, Vector3.down, out floorHit, baseHeight / 2 + groundCheckDistance, groundMask)
          || Physics.Raycast(transform.position, Vector3.down, baseHeight / 2 + groundCheckDistance, groundMask);
        IsOnCeiling = Physics.SphereCast(transform.position, baseThickness, Vector3.up, out ceilingHit, baseHeight / 2 + ceilingCheckDistance, groundMask)
          || Physics.Raycast(transform.position, Vector3.up, baseHeight / 2 + ceilingCheckDistance, groundMask);
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
        DebugDrawer drawer = new DebugDrawer();

        drawer.DrawRaySphereCheck(transform.position, Vector3.down, baseHeight + groundCheckDistance, baseThickness, Color.red);
        drawer.DrawRaySphereCheck(transform.position, Vector2.up, baseHeight + ceilingCheckDistance, baseThickness, Color.red);
    }
    #endregion


}
