using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Ejemplo: Restar vida al enemigo
            EnemyHealth health = other.GetComponent<EnemyHealth>();
            if (health != null) health.TakeDamage(damage);
        }
    }
}