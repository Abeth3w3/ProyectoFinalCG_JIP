using UnityEngine;

public class GluttonyEnemy : MonoBehaviour
{
    public System.Action<GluttonyEnemy> onEnemyDeath;
    public int rutina;
    public float cronometro;
    public Animator animator;
    public Quaternion angulo;
    public float grado;

    public Transform target;
    public bool attack;

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

    private float attackTimer = 0f;
    private bool playerDetected = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                target = player.transform;
        }
    }

    void Update()
    {
        if (currentHealth > 0 && target != null)
        {
            Comportamiento_Enemigo();
        }
    }

    // ===== COMPORTAMIENTO PRINCIPAL =====
    public void Comportamiento_Enemigo()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Detecta sin necesidad de visión exacta
        if (distanceToTarget <= detectionRange)
        {
            playerDetected = true;
            cronometro = 0;
        }
        else if (distanceToTarget > detectionRange * 1.5f)
        {
            playerDetected = false;
        }

        // ===== PATRULLA =====
        if (!playerDetected)
        {
            animator.SetBool("run", false);
            cronometro += Time.deltaTime;

            if (cronometro >= 4)
            {
                rutina = Random.Range(0, 3);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    animator.SetBool("walk", false);
                    break;

                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina = 2;
                    break;

                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1f * Time.deltaTime);
                    animator.SetBool("walk", true);
                    break;
            }

            return;
        }

        // ===== PERSECUCIÓN =====
        Vector3 lookPos = target.position - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2f);

        animator.SetBool("walk", false);

        // CORRER si no está en rango
        if (distanceToTarget > attackRange)
        {
            animator.SetBool("run", true);
            transform.Translate(Vector3.forward * 10f * Time.deltaTime);
        }
        else
        {
            animator.SetBool("run", false);

            // ATAQUE si está cerca y puede ver al jugador
            if (attackTimer <= 0 && CanSeePlayer())
            {
                AttackPlayer();
            }
        }
    }

    // ===== VISIÓN DEL ENEMIGO (CORREGIDA) =====
    bool CanSeePlayer()
    {
        if (target == null) return false;

        Vector3 eyePos = transform.position + Vector3.up * 1.6f; // altura correcta
        Vector3 direction = (target.position + Vector3.up * 1.2f) - eyePos;

        float angle = Vector3.Angle(transform.forward, direction);

        if (angle < visionAngle / 2f)
        {
            if (Physics.Raycast(eyePos, direction.normalized, out RaycastHit hit, detectionRange))
            {
                Debug.DrawRay(eyePos, direction.normalized * detectionRange, Color.red);

                if (hit.collider.CompareTag("Player"))
                    return true;
            }
        }

        return false;
    }

    // ===== ATAQUE =====
    void AttackPlayer()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            animator.SetTrigger("attack"); // AHORA SÍ SE ACTIVA

            PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
            if (playerHealth != null)
                playerHealth.TakeDamage((int)attackDamage);

            attackTimer = attackCooldown;
        }
    }

    public void OnAttackAnimationEvent()
    {
        Debug.Log("🗡️ Animación de ataque ejecutada");
    }

    // ===== DAÑO =====
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        animator.SetTrigger("hit");

        // Forzar detección al ser golpeado
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
            playerDetected = true;
        }

        if (currentHealth <= 0)
            Die();
    }

    // ===== MUERTE =====
    void Die()
    {
        onEnemyDeath?.Invoke(this);

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        this.enabled = false;
        animator.SetTrigger("die");

        Destroy(gameObject, 3f);
    }

    // ===== DEBUG =====
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;
        Vector3 left = Quaternion.Euler(0, -visionAngle / 2, 0) * transform.forward * detectionRange;
        Vector3 right = Quaternion.Euler(0, visionAngle / 2, 0) * transform.forward * detectionRange;

        Gizmos.DrawRay(transform.position, left);
        Gizmos.DrawRay(transform.position, right);
    }
}
