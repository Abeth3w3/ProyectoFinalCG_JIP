using UnityEngine;
using System.Collections;

public class BossSimpleAI : MonoBehaviour
{
    [Header("Boss Settings")]
    public float normalSpeed = 2f;
    public float speedBurstMultiplier = 1.8f; // Multiplicador durante las ráfagas
    public float chaseRange = 15f;
    public float minDistanceToPlayer = 2f;
    public float attackRange = 2f; // Nuevo: rango para atacar

    [Header("Damage Settings")]
    public int meleeDamage = 20;
    public float damageCooldown = 2f;

    [Header("Speed Burst Settings")]
    public float minBurstInterval = 3f;
    public float maxBurstInterval = 7f;
    public float burstDuration = 1.2f;

    private Transform player;
    private EnemyHealth health;
    private float currentSpeed;
    private bool phase2Activated = false;

    // Variables para ráfagas de velocidad
    private bool isSpeedBurstActive = false;
    private float nextBurstTime = 0f;
    private float lastDamageTime = 0f;
    private float originalSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        health = GetComponent<EnemyHealth>();
        currentSpeed = normalSpeed;
        originalSpeed = normalSpeed;

        // Establecer el primer tiempo para una ráfaga de velocidad
        nextBurstTime = Time.time + Random.Range(minBurstInterval, maxBurstInterval);
    }

    void Update()
    {
        if (player == null) return;

        // Verificar cambio de fase al 50%
        if (!phase2Activated && health.GetCurrentHealth() <= health.maxHealth / 2)
        {
            ActivatePhase2();
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            // Comportamiento normal de persecución
            if (distanceToPlayer > minDistanceToPlayer)
            {
                ChasePlayer();
            }
            else
            {
                RotateTowardsPlayer();
            }

            // Manejar ráfagas de velocidad en fase 2
            if (phase2Activated)
            {
                HandleSpeedBursts();
            }

            // Ataque por proximidad
            if (distanceToPlayer <= attackRange && Time.time >= lastDamageTime + damageCooldown)
            {
                DealDamage();
            }
        }
    }

    void ActivatePhase2()
    {
        phase2Activated = true;
        Debug.Log("¡BOSS ENFURECIDO! Fase 2 activada - ¡Cuidado con sus ráfagas de velocidad!");

        // Pequeño efecto visual
        StartCoroutine(EnrageEffect());
    }

    IEnumerator EnrageEffect()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.magenta;
            yield return new WaitForSeconds(1.5f);
            renderer.material.color = originalColor;
        }
    }

    void HandleSpeedBursts()
    {
        // Verificar si es tiempo de activar una ráfaga de velocidad
        if (!isSpeedBurstActive && Time.time >= nextBurstTime)
        {
            StartCoroutine(SpeedBurst());
        }
    }

    IEnumerator SpeedBurst()
    {
        isSpeedBurstActive = true;

        // Activar ráfaga de velocidad
        currentSpeed = originalSpeed * speedBurstMultiplier;

        // Efecto visual durante la ráfaga
        Renderer renderer = GetComponent<Renderer>();
        Color originalColor = Color.white;
        if (renderer != null)
        {
            originalColor = renderer.material.color;
            renderer.material.color = Color.yellow;
        }

        Debug.Log("¡BOSS ACELERA!");

        // Duración de la ráfaga
        yield return new WaitForSeconds(burstDuration);

        // Volver a velocidad normal
        currentSpeed = originalSpeed;

        // Restaurar color
        if (renderer != null)
            renderer.material.color = originalColor;

        // Programar próxima ráfaga
        nextBurstTime = Time.time + Random.Range(minBurstInterval, maxBurstInterval);
        isSpeedBurstActive = false;
    }

    void ChasePlayer()
    {
        // Moverse hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;
        transform.position += direction * currentSpeed * Time.deltaTime;

        RotateTowardsPlayer();
    }

    void RotateTowardsPlayer()
    {
        // Rotar hacia el jugador
        Vector3 direction = (player.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Vector3 lookDirection = new Vector3(direction.x, 0, direction.z);
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }

    void DealDamage()
    {
        lastDamageTime = Time.time;

        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(meleeDamage);
            Debug.Log("Daño del boss");

            // Empujar al jugador si está en ráfaga de velocidad
            if (isSpeedBurstActive)
            {
                Rigidbody playerRb = player.GetComponent<Rigidbody>();
                if (playerRb != null)
                {
                    Vector3 pushDirection = (player.transform.position - transform.position).normalized;
                    playerRb.AddForce(pushDirection * 12f, ForceMode.Impulse);
                    Debug.Log("¡Empujón extra por ráfaga de velocidad!");
                }
            }
        }
    }

    // También mantener las colisiones por si acaso
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            DealDamage();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            DealDamage();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar rango de persecución
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // Visualizar distancia mínima
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistanceToPlayer);

        // Visualizar rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Visualizar estado de ráfaga de velocidad
        if (isSpeedBurstActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 3f);
        }
    }
}