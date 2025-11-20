using UnityEngine;

public class EnemyDamageFinal2 : MonoBehaviour
{
    [Header("Configuración de Daño")]
    public int damage = 10;
    public float cooldown = 1f;

    private bool canDamage = true;

    void Start()
    {
        Debug.Log("EnemyDamageFinal iniciado. Daño: " + damage);
    }

    void OnCollisionEnter(Collision collision)
    {
        TryToDamage(collision.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        TryToDamage(collision.gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        TryToDamage(other.gameObject);
    }

    void TryToDamage(GameObject target)
    {
        if (!canDamage || !target.CompareTag("Player")) return;

        PlayerHealthFinal playerHealth = target.GetComponent<PlayerHealthFinal>();

        if (playerHealth != null)
        {
            if (!playerHealth.isDead)
            {
                playerHealth.TakeDamage(damage);
                canDamage = false;

                Debug.Log("¡Daño aplicado! " + damage + " de daño. Vida restante: " + playerHealth.currentHealth);

                Invoke("ResetDamage", cooldown);
            }
            else
            {
                Debug.Log("El jugador ya está muerto, no se puede hacer más daño");
            }
        }
        else
        {
            Debug.LogError("No se encontró PlayerHealthFinal en el jugador");
        }
    }

    void ResetDamage()
    {
        canDamage = true;
    }

    // Para debugging
    void OnGUI()
    {
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, 40, 300, 30), "Enemigo - Puede dañar: " + canDamage);
    }
}