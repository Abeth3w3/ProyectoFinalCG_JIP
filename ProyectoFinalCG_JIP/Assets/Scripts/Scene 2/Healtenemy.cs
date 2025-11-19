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

    [Header("Color Damage System")]
    public bool enableColorChange = true;
    public Color fullHealthColor = Color.white;
    public Color lowHealthColor = Color.red;

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

        UpdateHealthColor();
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

        // Actualizar color según salud
        UpdateHealthColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        isFlashing = true;

        // Guardar color actual antes del flash
        Color[] currentColors = new Color[enemyRenderers.Length];
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i].material != null)
            {
                currentColors[i] = enemyRenderers[i].material.color;
                enemyRenderers[i].material.color = flashColor;
            }
        }

        // Esperar
        yield return new WaitForSeconds(flashDuration);

        // Volver a los colores según salud actual
        UpdateHealthColor();

        isFlashing = false;
    }

    void UpdateHealthColor()
    {
        if (!enableColorChange) return;

        // Calcular progreso del color (0 = salud llena, 1 = sin salud)
        float healthPercent = (float)currentHealth / maxHealth;
        Color targetColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);

        // Aplicar color a todos los renderers
        for (int i = 0; i < enemyRenderers.Length; i++)
        {
            if (enemyRenderers[i].material != null)
            {
                enemyRenderers[i].material.color = targetColor;
            }
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " murió");

        // Efecto de muerte
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Si es el boss, notificar al GameManager
        if (gameObject.CompareTag("Boss") && GameManager.Instance != null)
        {
            GameManager.Instance.BossDefeated();
        }

        Destroy(gameObject);
    }

    // MÉTODO NUEVO QUE FALTABA - Esto soluciona el error
    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}