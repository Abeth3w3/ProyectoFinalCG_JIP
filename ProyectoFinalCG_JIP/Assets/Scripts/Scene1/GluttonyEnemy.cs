using UnityEngine;

public class GluttonyEnemy : MonoBehaviour
{
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

    [Header("Ataque")]
    public float attackDamage = 15f;
    public float attackCooldown = 2f;
    private float attackTimer = 0f;
    public float attackRange = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    public void Comportamiento_Enemigo()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Si está lejos del jugador (modo patrulla)
        if (distanceToTarget > 5)
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
        }
        else
        {
            // MODO PERSECUCIÓN
            Vector3 lookPos = target.position - transform.position;
            lookPos.y = 0;

            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 2f);

            animator.SetBool("walk", false);

            // Solo correr si no está en rango de ataque
            if (distanceToTarget > attackRange)
            {
                animator.SetBool("run", true);
                transform.Translate(Vector3.forward * 3f * Time.deltaTime);
            }
            else
            {
                animator.SetBool("run", false);

                // Atacar si está en rango
                if (attackTimer <= 0)
                {
                    AttackPlayer();
                }
            }
        }
    }

    // Ataque al jugador
    void AttackPlayer()
    {
        animator.SetTrigger("attack");
        PlayerHealth playerHealth = target.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage((int)attackDamage);
        }

        attackTimer = attackCooldown;
        Debug.Log("👊 Enemigo ataca al jugador");
    }

    // Recibir daño (ESTE MÉTODO USA LA HAMBURGUESA)
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"🎯 Enemigo recibe {damage} daño. HP: {currentHealth}/{maxHealth}");

        // Animación de daño
        animator.SetTrigger("hit");

        // Muerte
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Muerte del enemigo
    void Die()
    {
        Debug.Log("☠️ Enemigo derrotado");

        if (deathSound != null)
            AudioSource.PlayClipAtPoint(deathSound, transform.position);

        if (deathParticles != null)
            Instantiate(deathParticles, transform.position, Quaternion.identity);

        // Desactivar colisiones y script
        GetComponent<Collider>().enabled = false;
        this.enabled = false;

        // Animación de muerte
        animator.SetTrigger("die");

        // Destruir después de un tiempo
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        if (currentHealth > 0) // Solo ejecutar si está vivo
        {
            Comportamiento_Enemigo();
        }
    }

    // Debug visual de rangos
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5f); // Rango de detección
    }
}