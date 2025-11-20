using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 30;
    public GameObject deathEffect;

    [Header("Damage Visual Effect")]
    public float flashDuration = 0.3f;

    private int currentHealth;
    private Renderer[] enemyRenderers;
    private Color[] originalColors;
    private bool isFlashing = false;

    void Start()
    {
        currentHealth = maxHealth;

        // Obtener todos los renderers del enemigo (incluyendo hijos)
        enemyRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[enemyRenderers.Length];

        // Guardar colores originales
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material != null)
            {
                originalColors[i] = enemyRenderers[i].material.color;
            }
        }

        Debug.Log($"Enemigo listo. Renderers encontrados: {enemyRenderers.Length}");
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} recibió {damage} de daño. Vida: {currentHealth}");

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

        // Aplicar color rojo a todos los renderers
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material != null)
            {
                enemyRenderers[i].material.color = Color.red;
            }
        }

        // Esperar
        yield return new WaitForSeconds(flashDuration);

        // Restaurar colores originales
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i] != null && enemyRenderers[i].material != null)
            {
                enemyRenderers[i].material.color = originalColors[i];
            }
        }

        isFlashing = false;
    }

    void Die()
    {
        Debug.Log($"{gameObject.name} murió");

        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}