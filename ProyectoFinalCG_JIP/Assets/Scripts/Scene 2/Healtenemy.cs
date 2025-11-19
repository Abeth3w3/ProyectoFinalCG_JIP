using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 30;
    public GameObject deathEffect;

    [Header("Damage Visual Effect")]
    public float flashDuration = 0.2f;
    public Color flashColor = Color.red;

    private int currentHealth;
    private Renderer[] enemyRenderers;
    private Color[] originalColors;
    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Obtener todos los renderers en el objeto y sus hijos
        enemyRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[enemyRenderers.Length];

        // Guardar los colores originales
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i].material != null)
            {
                originalColors[i] = enemyRenderers[i].material.color;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemigo recibió " + damage + " de daño. Vida: " + currentHealth);

        // Efecto visual de daño
        if (!isFlashing)
        {
            StartCoroutine(DamageFlash());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        isFlashing = true;

        // Cambiar a color de flash
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i].material != null)
            {
                enemyRenderers[i].material.color = flashColor;
            }
        }

        // Esperar
        yield return new WaitForSeconds(flashDuration);

        // Volver a los colores originales
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i].material != null)
            {
                enemyRenderers[i].material.color = originalColors[i];
            }
        }

        isFlashing = false;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió");

        // Efecto de muerte
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // AÑADIR ESTE MÉTODO PARA OBTENER LA SALUD ACTUAL
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}