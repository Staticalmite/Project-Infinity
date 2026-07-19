using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public Waypoint[] nextWaypoint;


    // This method is called by Unity to draw gizmos in the editor, makes easy to find and visualize waypoints and their connections
    private void OnDrawGizmos()
    {
        // Draw a clean, bright sphere at the waypoint's position
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, 0.6f); 

        // Draw lines connecting this waypoint to all its next targets
        if (nextWaypoint != null)
        {
            Gizmos.color = Color.yellow;
            foreach (Waypoint next in nextWaypoint)
            {
                if (next != null)
                {
                    // Draws a line from this waypoint to the next one
                    Gizmos.DrawLine(transform.position, next.transform.position);
                }
            }
        }
    }
}
