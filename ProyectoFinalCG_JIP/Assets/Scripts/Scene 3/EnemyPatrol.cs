using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public Transform[] waypoints;
    public float walkSpeed = 2f;
    public float waitTime = 2f;
    int currentIndex = 0;
    float waitCounter;

    [Header("Detection Settings")]
    public Transform visionPivot;
    public float viewDistance = 8f;
    public float viewAngle = 60f;

    public Transform player;

    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed;
        agent.autoBraking = false;
        GoToNextPoint();
    }

    void Update()
    {
        // Patrulla
        if (agent.isOnNavMesh)
        {
            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                waitCounter += Time.deltaTime;
                if (waitCounter >= waitTime)
                {
                    GoToNextPoint();
                    waitCounter = 0f;
                }
            }
        }


        // Detección Visual
        if (CanSeePlayer())
        {
            TriggerDetection();
        }
    }

    void GoToNextPoint()
    {
        if (waypoints.Length == 0) return;
        agent.destination = waypoints[currentIndex].position;
        currentIndex = (currentIndex + 1) % waypoints.Length;
    }

    bool CanSeePlayer()
    {
        Vector3 dir = (player.position - visionPivot.position).normalized;
        float angle = Vector3.Angle(visionPivot.forward, dir);

        if (angle < viewAngle / 2f)
        {
            if (Vector3.Distance(visionPivot.position, player.position) <= viewDistance)
            {
                // Línea de visión sin obstáculos
                if (!Physics.Linecast(visionPivot.position, player.position))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void TriggerDetection()
    {
        Debug.Log("Jugador detectado → Cinemática + Reinicio");
        // Aquí después llamamos a la cinemática y reinicio
    }
}
