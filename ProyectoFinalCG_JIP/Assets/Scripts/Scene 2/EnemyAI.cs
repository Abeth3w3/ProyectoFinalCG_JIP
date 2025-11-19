using UnityEngine;
using UnityEngine.AI;

public class SimpleEnemyAI : MonoBehaviour
{
    [Header("AI Settings")]
    public float detectionRange = 10f;
    public float stoppingDistance = 1f;

    private NavMeshAgent agent;
    private Transform player;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Detectar jugador
        if (distance <= detectionRange)
        {
            if (!isChasing)
            {
                isChasing = true;
                Debug.Log("Enemigo empezó a perseguirte!");
            }

            // Perseguir jugador
            agent.SetDestination(player.position);
        }
        else
        {
            if (isChasing)
            {
                isChasing = false;
                agent.ResetPath();
                Debug.Log("Enemigo perdió el rastro");
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Rango de detección (amarillo)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Rango de contacto (rojo)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}