using UnityEngine;

public class EnemyDamageFinal : MonoBehaviour
{
    public int damage = 10;
    public float cooldown = 1f;
    private bool canDamage = true;

    void OnCollisionEnter(Collision collision)
    {
        DamagePlayer(collision.gameObject);
    }

    void OnCollisionStay(Collision collision)
    {
        DamagePlayer(collision.gameObject);
    }

    void DamagePlayer(GameObject player)
    {
        if (!canDamage || !player.CompareTag("Player")) return;

        PlayerHealthFinal health = player.GetComponent<PlayerHealthFinal>();
        if (health != null && !health.isDead) // ✅ Usando la variable simple
        {
            health.TakeDamage(damage);
            canDamage = false;
            Invoke("ResetDamage", cooldown);
        }
    }

    void ResetDamage()
    {
        canDamage = true;
    }
}