using UnityEngine;

public class DebugDrawer 
{
    public void DrawRaySphereCheck(Vector3 origin, Vector3 direction, float length, float thickness, Color color)
    {
        Gizmos.color = color;
        // Draw the ray as a line
        Gizmos.DrawLine(origin, origin + direction * length);

        // Optionally draw a sphere at the hit point for better visibility
        Gizmos.DrawSphere(origin + direction * length, thickness);
    }

    public void DrawRayCheck(Vector3 origin, Vector3 direction, float length, Color color )
    {
        Gizmos.color = color;
        // Draw the ray as a line
        Gizmos.DrawLine(origin, origin + direction * length);
    }
}
