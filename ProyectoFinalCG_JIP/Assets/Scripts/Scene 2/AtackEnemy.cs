using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public int contactDamage = 10;
    public float damageCooldown = 2f;
    public float contactRange = 1.5f;

    [Header("Visual Feedback")]
    public ParticleSystem damageParticles;
    public AudioClip damageSound;

    private float lastDamageTime = 0f;
    private Transform player;
    private PlayerHealth playerHealth;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    void Update()
    {
        if (player == null || playerHealth == null) return;

        // Verificar distancia al jugador
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= contactRange && Time.time >= lastDamageTime + damageCooldown)
        {
            DealContactDamage();
        }
    }

    void DealContactDamage()
    {
        lastDamageTime = Time.time;

        // Aplicar daño al jugador
        playerHealth.TakeDamage(contactDamage);

        // Efectos visuales y de sonido
        if (damageParticles != null)
        {
            Instantiate(damageParticles, player.position, Quaternion.identity);
        }

        if (damageSound != null)
        {
            AudioSource.PlayClipAtPoint(damageSound, transform.position);
        }

        Debug.Log($"Enemigo hizo {contactDamage} de daño por contacto");
    }

    // Opcional: También puedes usar triggers para mayor precisión
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Time.time >= lastDamageTime + damageCooldown)
        {
            DealContactDamage();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, contactRange);
    }
}