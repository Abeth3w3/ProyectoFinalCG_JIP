using UnityEngine;
using UnityEngine.AI;

public class EnemyFollower : MonoBehaviour
{
    [Header("Configuración de Seguimiento")]
    public Transform playerTarget;
    public float followRange = 15f;
    public float stoppingDistance = 2f;
    public float searchPlayerInterval = 1f;

    [Header("Componentes")]
    private NavMeshAgent agent;
    private Animator animator;

    private float lastSearchTime;
    private bool hasValidPath;

    void Start()
    {
        // Obtener componentes
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        // Configurar agente
        if (agent != null)
        {
            agent.stoppingDistance = stoppingDistance;
            agent.autoBraking = true;
        }

        // Buscar jugador inicial
        FindPlayer();

        Debug.Log("EnemyFollower iniciado. Agent: " + (agent != null));
    }

    void Update()
    {
        // Buscar jugador periódicamente si no se tiene
        if (playerTarget == null && Time.time - lastSearchTime > searchPlayerInterval)
        {
            FindPlayer();
            lastSearchTime = Time.time;
        }

        // Seguir al jugador si existe
        if (playerTarget != null)
        {
            FollowPlayer();
        }

        // Actualizar animaciones
        UpdateAnimations();
    }

    void FollowPlayer()
    {
        if (agent == null || !agent.isOnNavMesh || !agent.isActiveAndEnabled)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        // Solo seguir si está en rango y no está demasiado cerca
        if (distanceToPlayer <= followRange && distanceToPlayer > stoppingDistance)
        {
            agent.SetDestination(playerTarget.position);
            hasValidPath = agent.pathStatus == NavMeshPathStatus.PathComplete;
        }
        else if (distanceToPlayer > followRange)
        {
            // Si está muy lejos, dejar de seguir
            agent.ResetPath();
            hasValidPath = false;
        }
    }

    void FindPlayer()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerTarget = playerObj.transform;
            Debug.Log("Jugador encontrado: " + playerObj.name);
        }
        else
        {
            Debug.LogWarning("Buscando jugador... (asegúrate de que tiene tag 'Player')");
        }
    }

    void UpdateAnimations()
    {
        if (animator == null) return;

        bool isMoving = agent != null && agent.velocity.magnitude > 0.1f;
        animator.SetBool("IsMoving", isMoving);

        // Si tienes otras animaciones, agrégalas aquí
        // animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    // Método para asignar jugador manualmente
    public void SetPlayerTarget(Transform newTarget)
    {
        playerTarget = newTarget;
    }

    // Visualizar en el Editor
    private void OnDrawGizmosSelected()
    {
        // Rango de seguimiento
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, followRange);

        // Distancia de parada
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);

        // Línea hacia el jugador
        if (playerTarget != null)
        {
            Gizmos.color = hasValidPath ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, playerTarget.position);
        }
    }
}