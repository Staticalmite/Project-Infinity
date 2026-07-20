using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Patrolling,
        Chasing,
    }

    private EnemyState currentState = EnemyState.Patrolling;
    private Transform player;

    private NavMeshAgent agent;
    [SerializeField] private Waypoint currentWaypoint;

    [Header("DetectionSettings")]
    [SerializeField] private float detectionRange = 5f;
    [SerializeField] private float loseSightRange = 8f; // Slightly larger to prevent jittering

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = FindFirstObjectByType<FirstPersonController>().transform;

        // Make sure a waypoint was assigned in the inspector
        if (currentWaypoint != null)
        {
            agent.SetDestination(currentWaypoint.transform.position);
        }
    }

    void Update()
    {

        switch (currentState)
        {
            case EnemyState.Patrolling:
                UpdatePatrolState();
                break;
            case EnemyState.Chasing:
                UpdateChaseState();
                break;
        }
    }

    private void UpdateChaseState()
    {
        // Check for transition back to Patrolling
        if (player == null || Vector3.Distance(transform.position, player.position) > loseSightRange)
        {
            currentState = EnemyState.Patrolling;

            // Resume pathing toward the last known waypoint
            if (currentWaypoint != null)
            {
                agent.SetDestination(currentWaypoint.transform.position);
            }
            return;
        }

        // Standard Chase Logic
        agent.SetDestination(player.position);
    }


    // State Logic 
    private void UpdatePatrolState()
    {
        // Check for transition to Chasing
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            currentState = EnemyState.Chasing;
            return; // Exit the method immediately so patrol logic doesn't run
        }

        // Standard Patrol Logic
        if (currentWaypoint == null) return;

        if (!agent.pathPending && agent.remainingDistance < 1.5f)
        {
            SetNextRandomWaypoint();
        }
    }
    private void SetNextRandomWaypoint()
    {
        // Ensure the current waypoint actually has paths to choose from
        if (currentWaypoint.nextWaypoint == null || currentWaypoint.nextWaypoint.Length == 0)
        {
            Debug.LogWarning($"EnemyAI: {currentWaypoint.name} has no outbound connections configured!");
            return;
        }

        // Choose a random next waypoint from the array of Waypoints attached to the current waypoint
        int randomIndex = Random.Range(0, currentWaypoint.nextWaypoint.Length);
        Waypoint targetWaypoint = currentWaypoint.nextWaypoint[randomIndex];

        if (targetWaypoint != null)
        {
            currentWaypoint = targetWaypoint;
            agent.SetDestination(currentWaypoint.transform.position);
        }
    }

    // Visual aid for tuning the detection ranges in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseSightRange);
    }
}