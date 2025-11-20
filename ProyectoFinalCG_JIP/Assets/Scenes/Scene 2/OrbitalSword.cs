using UnityEngine;
using System.Collections;

public class OrbitalSword : MonoBehaviour
{
    [Header("Orbital Settings")]
    public Transform playerCenter;
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 200f;
    public float orbitHeight = 1.0f;

    [Header("Damage Settings")]
    public int damage = 15;
    public float damageRadius = 1.0f;
    public float attackCooldown = 0.3f;

    [Header("Sword Rotation")]
    public Vector3 swordRotation = new Vector3(90, 0, 0);

    [Header("Effects")]
    public ParticleSystem hitEffect;
    public AudioClip hitSound;

    private float currentAngle = 0f;
    private bool isActive = false;
    private Vector3 orbitCenter;
    private float lastAttackTime = 0f;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (playerCenter != null && isActive)
        {
            OrbitAroundPlayer();
            CheckForEnemies();
        }
    }

    void OrbitAroundPlayer()
    {
        // Calcular el centro de órbita
        orbitCenter = playerCenter.position + Vector3.up * orbitHeight;

        // Calcular nueva posición orbital
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        float rad = currentAngle * Mathf.Deg2Rad;

        // Posición orbital alrededor del personaje
        Vector3 orbitPosition = new Vector3(
            Mathf.Cos(rad) * orbitRadius,
            0,
            Mathf.Sin(rad) * orbitRadius
        );

        // Aplicar posición
        transform.position = orbitCenter + orbitPosition;

        // Rotar la espada para que esté ACOSTADA
        Vector3 tangent = new Vector3(-Mathf.Sin(rad), 0, Mathf.Cos(rad));
        transform.rotation = Quaternion.LookRotation(tangent, Vector3.up) * Quaternion.Euler(swordRotation);
    }

    void CheckForEnemies()
    {
        if (Time.time < lastAttackTime + attackCooldown) return;

        // Buscar enemigos en el radio de daño
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Enemy"))
            {
                AttackEnemy(hit.gameObject);
                break; // Solo atacar a un enemigo por frame
            }
        }
    }

    void AttackEnemy(GameObject enemy)
    {
        lastAttackTime = Time.time;

        // CORREGIDO: Buscar SimpleEnemy en lugar de PlayerHealthFinal
        SimpleEnemy enemyHealth = enemy.GetComponent<SimpleEnemy>();
        if (enemyHealth != null && !enemyHealth.isDead)
        {
            enemyHealth.TakeDamage(damage);
            Debug.Log("⚔️ Espada orbital hizo " + damage + " de daño a " + enemy.name);

            // Efectos de golpe
            PlayHitEffects(enemy.transform.position);
        }
        else
        {
            Debug.Log("Enemigo no tiene componente SimpleEnemy o está muerto");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // También detectar enemigos por trigger (backup)
        if (other.CompareTag("Enemy") && isActive && Time.time >= lastAttackTime + attackCooldown)
        {
            AttackEnemy(other.gameObject);
        }
    }

    void PlayHitEffects(Vector3 position)
    {
        // Efecto de partículas
        if (hitEffect != null)
        {
            Instantiate(hitEffect, position, Quaternion.identity);
        }

        // Sonido de golpe
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    public void SetOrbitCenter(Transform center)
    {
        playerCenter = center;
        isActive = true;
        currentAngle = Random.Range(0f, 360f);
    }

    public void SetOrbitHeight(float height)
    {
        orbitHeight = height;
    }

    public void SetSwordRotation(Vector3 rotation)
    {
        swordRotation = rotation;
    }

    // Debug visual
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRadius);

        // Mostrar dirección de ataque
        if (isActive)
        {
            Gizmos.color = Color.blue;
            Vector3 tangent = new Vector3(-Mathf.Sin(currentAngle * Mathf.Deg2Rad), 0, Mathf.Cos(currentAngle * Mathf.Deg2Rad));
            Gizmos.DrawRay(transform.position, tangent * 2f);
        }
    }
}