using System.Collections;
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
    private FirstPersonController firstPersonController;

    private NavMeshAgent agent;
    [SerializeField] private Waypoint currentWaypoint;

    [Header("DetectionSettings")]
    [SerializeField] private float detectionRange;
    [SerializeField] private float loseSightRange;

    private LayerMask layerMask;
    private RaycastHit hit;
    private EnemyAudioManager audioManager;


    public AudioSource audioSource;


    private Animator animator; 

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        firstPersonController = FindFirstObjectByType<FirstPersonController>();
        player = firstPersonController.transform;
        audioSource = GetComponent<AudioSource>();
        audioManager = FindFirstObjectByType<EnemyAudioManager>();
        animator = GetComponent<Animator>();


        layerMask = LayerMask.GetMask("Wall", "Player");

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
        Debug.DrawRay(transform.position, (firstPersonController.transform.position - transform.position).normalized * detectionRange, Color.blue, 0.1f);
        // Check for transition to Chasing - if the player is making noise and in sight, switch to Chasing.
        if (firstPersonController != null && firstPersonController.makingNoise && Physics.Raycast(transform.position, player.position - transform.position, out hit, detectionRange, layerMask))
        {
            if(hit.collider.CompareTag("Player"))
            {
                print(hit);
                currentState = EnemyState.Chasing;
                animator.SetBool("isChasing", true);
                // To get the right length for the delay - delay should be the length of the animation divided by the speed of the animation.
                // For example, if the animation is 1.208 seconds, then the delay should be 1.208 / 0.5 = 2.416 seconds.
                StartCoroutine(StartChaseRoutine(2.416f));
                return; // Exit the method immediately so patrol logic doesn't run
            }
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

    private IEnumerator StartChaseRoutine(float delay)
    {
        // Stop the enemy immediately & play the scream audio.
        agent.speed = 0;
        audioManager.PlayScream();

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Speed the enemy back up and play the chase audio.
        audioManager.PlayChaseAudio();
        agent.speed = 4;
    }
}