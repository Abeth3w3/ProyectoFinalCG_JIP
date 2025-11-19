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

    private float currentAngle = 0f;
    private bool isActive = false;
    private Vector3 orbitCenter;
    private float lastAttackTime = 0f;

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
        // Buscar enemigos en el radio de daño
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, damageRadius);

        // LOG: Mostrar cuántos objetos se detectaron
        if (hitColliders.Length > 0)
        {
            Debug.Log($"Se detectaron {hitColliders.Length} objetos en el radio de daño");
        }

        foreach (Collider hit in hitColliders)
        {
            // LOG: Verificar qué objeto se está detectando
            Debug.Log($"Objeto detectado: {hit.gameObject.name}, Tag: {hit.tag}");

            if (hit.CompareTag("Enemy") && Time.time >= lastAttackTime + attackCooldown)
            {
                // LOG: Confirmar que se identificó como enemigo
                Debug.Log($"Enemigo detectado: {hit.gameObject.name}");
                AttackEnemy(hit.gameObject);
            }
        }
    }

    void AttackEnemy(GameObject enemy)
    {
        lastAttackTime = Time.time;

        // LOG: Inicio del ataque
        Debug.Log($"Intentando atacar a: {enemy.name}");

        // Aplicar daño al enemigo
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();

        // LOG: Verificar si se encontró el componente EnemyHealth
        if (enemyHealth != null)
        {
            Debug.Log($"Componente EnemyHealth encontrado en {enemy.name}");
            enemyHealth.TakeDamage(damage);
            Debug.Log($"¡Daño aplicado! {damage} de daño a {enemy.name}");
        }
        else
        {
            Debug.LogError($"NO se encontró EnemyHealth en {enemy.name}");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // También detectar enemigos por trigger (backup)
        if (other.CompareTag("Enemy") && isActive && Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log($"Trigger detectado con: {other.gameObject.name}");
            AttackEnemy(other.gameObject);
        }
    }

    public void SetOrbitCenter(Transform center)
    {
        playerCenter = center;
        isActive = true;
        currentAngle = Random.Range(0f, 360f);
        Debug.Log("Órbita activada - Espada lista para atacar");
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
        if (isActive)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }
    }
}