using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{

    private NavMeshAgent agent;
    [SerializeField] private Waypoint currentWaypoint;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(currentWaypoint.transform.position);
    }
    
    void Update()
    {
        if (Vector3.Distance(transform.position, currentWaypoint.transform.position) < 3f)
        {
            // Choose a random next waypoint
            int randomIndex = Random.Range(0, currentWaypoint.nextWaypoint.Length);
            currentWaypoint = currentWaypoint.nextWaypoint[randomIndex];
            agent.SetDestination(currentWaypoint.transform.position);
        }
    }
}
