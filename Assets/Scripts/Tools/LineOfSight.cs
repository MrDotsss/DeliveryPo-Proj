using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LineOfSight : MonoBehaviour
{
    [Header("Vision Settings")]
    public float viewDistance = 10f;
    [Range(0, 360)] public float viewAngle = 90f;
    public LayerMask obstructionMask;
    public Transform eyeOrigin;

    [Tooltip("Optional. If empty, will default to player tag.")]
    public string targetTag = "Player";

    private Transform target;

    private void Start()
    {
        if (eyeOrigin == null) eyeOrigin = transform;

        if (string.IsNullOrEmpty(targetTag))
            target = GameObject.FindGameObjectWithTag("Player")?.transform;
        else
            target = GameObject.FindGameObjectWithTag(targetTag)?.transform;
    }

    public bool CanSeeTarget()
    {
        if (target == null) return false;

        return IsInRange(target) && IsInFOV(target) && !IsHidden(target);
    }

    public bool IsInRange(Transform target)
    {
        return Vector3.Distance(eyeOrigin.position, target.position) <= viewDistance;
    }

    public bool IsInFOV(Transform target)
    {
        Vector3 directionToTarget = (target.position - eyeOrigin.position).normalized;
        float angleBetween = Vector3.Angle(eyeOrigin.forward, directionToTarget);
        return angleBetween < viewAngle * 0.5f;
    }

    public bool IsHidden(Transform target)
    {
        Vector3 directionToTarget = (target.position - eyeOrigin.position).normalized;
        float distanceToTarget = Vector3.Distance(eyeOrigin.position, target.position);

        if (Physics.Raycast(eyeOrigin.position, directionToTarget, out RaycastHit hit, distanceToTarget, obstructionMask))
        {
            return hit.transform != target;
        }

        return false;
    }

    // Optional: Visualize LOS in Scene View
    private void OnDrawGizmosSelected()
    {
        if (eyeOrigin == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(eyeOrigin.position, viewDistance);

        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * eyeOrigin.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * eyeOrigin.forward;

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(eyeOrigin.position, rightBoundary * viewDistance);
        Gizmos.DrawRay(eyeOrigin.position, leftBoundary * viewDistance);

        if (target != null)
        {
            Gizmos.color = CanSeeTarget() ? Color.green : Color.red;
            Gizmos.DrawLine(eyeOrigin.position, target.position);
        }
    }
}
