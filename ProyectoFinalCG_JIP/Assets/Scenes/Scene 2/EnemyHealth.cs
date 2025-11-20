using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleEnemy : MonoBehaviour
{
    [Header("Sistema de Salud")]
    public int maxHealth = 50;
    public int currentHealth;
    public AudioClip deathSound;
    public ParticleSystem deathParticles;

    [Header("Detección y Ataque")]
    public float attackDamage = 15f;
    public float attackCooldown = 2f;
    public float attackRange = 2f;
    public float detectionRange = 8f;
    public float visionAngle = 120f;
    public float moveSpeed = 3f;

    [Header("Puntuación")]
    public int pointValue = 100;

    [Header("Cambio de Escena al Morir")]
    public string nextSceneName = "Scene3"; // Cambia esto por tu siguiente escena
    public float sceneChangeDelay = 3f;

    [Header("Estado")]
    public bool isDead = false; // CAMBIÉ ESTO A PÚBLICO

    private Transform target;
    private float attackTimer = 0f;
    private bool playerDetected = false;
    private Renderer enemyRenderer;
    private Color originalColor;

    // Propiedad pública para acceso externo (opcional - mejor práctica)
    public bool IsDead => isDead;

    void Start()
    {
        currentHealth = maxHealth;

        // Buscar jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;

        // Configurar renderer para efectos de daño
        enemyRenderer = GetComponentInChildren<Renderer>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }

        // Asegurar que tenga collider
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    void Update()
    {
        if (isDead) return;

        if (currentHealth > 0 && target != null)
        {
            Comportamiento_Enemigo();
        }
    }

    void Comportamiento_Enemigo()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Detectar jugador
        if (distanceToTarget <= detectionRange && CanSeePlayer())
        {
            playerDetected = true;
        }
        else if (distanceToTarget > detectionRange * 1.5f)
        {
            playerDetected = false;
        }

        if (playerDetected)
        {
            // Rotar hacia el jugador
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2f);

            // Moverse hacia el jugador si está lejos
            if (distanceToTarget > attackRange)
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            }
            else
            {
                // Atacar si está en rango
                if (attackTimer <= 0)
                {
                    AttackPlayer();
                }
            }
        }
        else
        {
            // Comportamiento de patrulla simple
            PatrolBehavior();
        }
    }

    void PatrolBehavior()
    {
        // Movimiento de patrulla básico
        transform.Translate(Vector3.forward * moveSpeed * 0.5f * Time.deltaTime);

        // Girar ocasionalmente
        if (Random.Range(0f, 1f) < 0.01f) // 1% de probabilidad por frame
        {
            transform.Rotate(0, Random.Range(-90f, 90f), 0);
        }
    }

    bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 direction = (target.position + Vector3.up * 1.2f) - transform.position;
        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < visionAngle / 2f)
        {
            if (Physics.Raycast(transform.position, direction.normalized, out RaycastHit hit, detectionRange))
            {
                Debug.DrawRay(transform.position, direction.normalized * detectionRange, Color.red);
                return hit.collider.CompareTag("Player");
            }
        }
        return false;
    }

    void AttackPlayer()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            // Aplicar daño al jugador
            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)attackDamage);
                Debug.Log($"⚔️ {gameObject.name} atacó al jugador: {attackDamage} de daño");
            }

            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"💥 {gameObject.name} recibió {damage} de daño. Vida: {currentHealth}/{maxHealth}");

        // Efecto visual de daño
        if (enemyRenderer != null)
        {
            StartCoroutine(FlashDamage());
        }

        // Alertar al enemigo sobre la posición del jugador
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            playerDetected = true;
        }

        if (currentHealth <= 0)
            Die();
    }

    System.Collections.IEnumerator FlashDamage()
    {
        if (enemyRenderer == null) yield break;

        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (enemyRenderer != null && !isDead)
            enemyRenderer.material.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"☠️ Enemigo derrotado: {gameObject.name}");

        // Efectos de muerte
        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        // Notificar al GameDataManager (opcional - para llevar registro)
        if (GameDataManager.Instance != null)
        {
            GameDataManager.Instance.AddEnemyKill();
            GameDataManager.Instance.AddScore(pointValue);
            Debug.Log($"🎯 Enemigo eliminado: +{pointValue} puntos");
        }

        // Desactivar componentes
        DisableEnemy();

        // Cambiar de escena después del delay (IGUAL QUE GLUTTONYENEMY)
        Invoke(nameof(ChangeToNextScene), sceneChangeDelay);

        // Destruir después de un tiempo
        Destroy(gameObject, sceneChangeDelay + 1f);
    }

    void ChangeToNextScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            Debug.Log("🔄 Cambiando a escena: " + nextSceneName);
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("⚠️ NextSceneName no está asignado en el inspector");
        }
    }

    void DisableEnemy()
    {
        // Desactivar renderer
        if (enemyRenderer != null)
            enemyRenderer.enabled = false;

        // Desactivar collider
        Collider collider = GetComponent<Collider>();
        if (collider != null)
            collider.enabled = false;

        // Desactivar scripts de IA/movimiento
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script.enabled)
            {
                script.enabled = false;
            }
        }

        // Desactivar Rigidbody si existe
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Detectar colisión con armas del jugador
        if (!isDead && other.CompareTag("Weapon"))
        {
            // Opcional: puedes agregar daño automático por trigger aquí
            // TakeDamage(10);
        }
    }

    void OnDrawGizmosSelected()
    {
        // Rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Rango de detección
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // Ángulo de visión
        Gizmos.color = Color.blue;
        Vector3 left = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * detectionRange;
        Vector3 right = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * detectionRange;

        Gizmos.DrawRay(transform.position, left);
        Gizmos.DrawRay(transform.position, right);
        Gizmos.DrawRay(transform.position, transform.forward * detectionRange);
    }
}