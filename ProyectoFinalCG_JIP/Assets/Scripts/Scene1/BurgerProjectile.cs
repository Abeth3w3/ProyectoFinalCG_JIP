using UnityEngine;

public class BurgerProjectile : MonoBehaviour
{
    [Header("Configuración")]
    public int damage = 15; // ← Cambiado a int
    public float lifetime = 3f;
    public GameObject impactEffect;

    void Start()
    {
        // Auto-destrucción después de tiempo
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Evitar colisión con el jugador que lanzó
        if (other.CompareTag("Player")) return;

        // Aplicar daño a enemigos
        if (other.CompareTag("Enemy"))
        {
            GluttonyEnemy enemy = other.GetComponent<GluttonyEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log("Hamburguesa hizo {damage} de daño a {other.name}");
            }
        }

        // Efecto de impacto (opcional)
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }

        // Destruir hamburguesa
        Destroy(gameObject);
    }
}