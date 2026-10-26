using System.Collections;
using System.ComponentModel.Design;
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

    // Adjust this value based on the length of the scream animation and its speed. First number is the speed, second is the speed.
    private float screamDelay = 1.208f / 0.5f;

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
                StartCoroutine(StartChaseRoutine(screamDelay));
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Handle player collision logic here
            Debug.Log("Player has been caught by the enemy!");
            // add logic for what happens when the player is caught, being reset to the start of the current task.
        }
    }

    public void Stun(float stunDuration)
    {
        // Handle logic for being stunned by the player, including for a normal stun and a counter stun.
        // This should include stopping the enemy, playing a stunned animation, and then resuming the previous state after a delay.
        agent.isStopped = true;
        animator.SetBool("isStunned", true);
        animator.SetBool("isChasing", false);

        // If the enemy is stunned and wasn't previously chasing, enter chase with the player as the target after a delay.
        StartCoroutine(StartChaseAfterStunRoutine(stunDuration));
    }

    // Visual aid for tuning the detection ranges in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, loseSightRange);
    }

    // Coroutine to handle the delay before starting the chase
    // Specifically makes sure the enemy stops moving, plays the scream audio, waits for the animation delay, then resumes chasing.
    private IEnumerator StartChaseRoutine(float delay)
    {
        // Stop the enemy immediately & play the scream audio.
        agent.isStopped = true;
        audioManager.PlayScream();

        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Resume the enemy, speed itup and play the chase audio.
        agent.isStopped = false;
        agent.speed = 4;
        audioManager.PlayChaseAudio();
    }

    // Coroutine to handle the delay before starting the chase after being stunned
    private IEnumerator StartChaseAfterStunRoutine(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);
        // Play the scream audio after the stun delay and start the scream delay.
        // If we're already chasing, instantiate the scream audio as to not interrupt the chase audio.
        if (currentState != EnemyState.Chasing)
        {
            audioManager.PlayScream();
        }
        else
        {
            audioManager.PlayScreamOneShot();
        }
            animator.SetBool("isStunned", false);
        animator.SetBool("isChasing", true);
        yield return new WaitForSeconds(screamDelay);
        // Resume the enemy, speed it up and play the chase audio.
        agent.isStopped = false;
        agent.speed = 4;
        // Only play the chase audio if we're not already chasing, to avoid overlapping audio.
        if (currentState != EnemyState.Chasing)
            audioManager.PlayChaseAudio();
        currentState = EnemyState.Chasing;
    }
}