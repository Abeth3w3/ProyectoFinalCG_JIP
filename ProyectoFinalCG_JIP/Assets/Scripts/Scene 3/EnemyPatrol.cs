using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] points;
    public float waitTimeAtPoint = 2f;

    [Header("Detection")]
    public float detectionRange = 6f;
    public Transform player;

    [Header("Animator")]
    public Animator animator;

    NavMeshAgent agent;
    int destPoint = 0;
    bool isWaiting = false;
    float waitTimer = 0f;
    bool playerDetected = false;
    



    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        if (agent == null) Debug.LogError("Missing NavMeshAgent on " + name);
    }

    void Start()
    {
        if (points == null || points.Length == 0)
        {
            Debug.LogWarning("No patrol points assigned on " + name);
            enabled = false;
            return;
        }

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        // ensure agent is active
        agent.isStopped = false;
        agent.enabled = true;

        // start patrol
        GoToNextPoint();
    }

    void Update()
    {
        // Safety: if agent not on NavMesh don't proceed
        if (!agent.isOnNavMesh)
        {
            // try to re-place on nearest position (optional) or warn
            if (!agent.isOnNavMesh) Debug.LogWarning(name + " not on NavMesh.");
            return;
        }

        DetectPlayer(); // updates playerDetected

        if (playerDetected)
        {
            // stop movement and set animation           
            UpdateAnimatorState(idle: false, walk: false, detected: true);
            return;

        }

        // if agent is stopped for other reason, resume
        if (agent.isStopped)
            agent.isStopped = false;

        // If we reached destination, begin waiting
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!isWaiting)
            {
                isWaiting = true;
                waitTimer = 0f;
                UpdateAnimatorState(idle: true, walk: false, detected: false);
            }
            else
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTimeAtPoint)
                {
                    isWaiting = false;
                    GoToNextPoint();
                }
            }
        }
        else
        {
            // moving
            UpdateAnimatorState(idle: false, walk: true, detected: false);
        }
    }

    void GoToNextPoint()
    {
        if (points.Length == 0) return;

        if (!agent.isOnNavMesh)
        {
            Debug.LogWarning(name + " GoToNextPoint called but agent not on NavMesh.");
            return;
        }

        agent.destination = points[destPoint].position;
        destPoint = (destPoint + 1) % points.Length;
        isWaiting = false;
        waitTimer = 0f;
        UpdateAnimatorState(idle: false, walk: true, detected: false);
        // debug
        Debug.Log(name + " -> Moving to " + agent.destination);
    }

    void DetectPlayer()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        bool inRange = dist <= detectionRange;

        if (inRange && !playerDetected)
        {
            playerDetected = true;
            Debug.Log(name + " detected player (range).");

           
        }
        


    }

    void UpdateAnimatorState(bool idle, bool walk, bool detected)
    {
        if (animator == null) return;

        // These parameter names must match exactly your Animator
        if (animator.HasParameterOfType("Idle", AnimatorControllerParameterType.Bool))
            animator.SetBool("Idle", idle);
        if (animator.HasParameterOfType("Walk", AnimatorControllerParameterType.Bool))
            animator.SetBool("Walk", walk);
        if (animator.HasParameterOfType("Detected", AnimatorControllerParameterType.Bool))
            animator.SetBool("Detected", detected);
        else if (detected)
        {
            // if Detected is a trigger instead of bool:
            if (animator.HasParameterOfType("Detected", AnimatorControllerParameterType.Trigger))
                animator.SetTrigger("Detected");
        }
    }
}

// Helper extension to test for parameter existence (keeps code safe)
public static class AnimatorExtensions
{
    public static bool HasParameterOfType(this Animator animator, string name, AnimatorControllerParameterType type)
    {
        foreach (var p in animator.parameters)
            if (p.name == name && p.type == type) return true;
        return false;
    }
    
}

