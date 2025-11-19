using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public Slider healthBar;
    public GameObject deathEffect;

    [Header("Color Damage Effect")]
    public bool enableColorChange = true;
    public Color fullHealthColor = Color.white;
    public Color lowHealthColor = Color.red;

    private int currentHealth;
    private Renderer[] playerRenderers;
    private Color[] originalColors;

    void Start()
    {
        currentHealth = maxHealth;

        // Obtener todos los renderers del jugador
        playerRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[playerRenderers.Length];

        // Guardar colores originales
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = playerRenderers[i].material.color;
            }
        }

        UpdateHealthUI();
        UpdateHealthColor();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Jugador recibió {damage} de daño. Vida: {currentHealth}");

        // Efecto de daño (flash rápido)
        StartCoroutine(DamageEffect());

        // Actualizar UI y color
        UpdateHealthUI();
        UpdateHealthColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator DamageEffect()
    {
        // Efecto visual de parpadeo
        foreach (Renderer renderer in playerRenderers)
        {
            if (renderer.material.HasProperty("_Color"))
            {
                renderer.material.color = Color.white; // Flash blanco
            }
        }

        yield return new WaitForSeconds(0.1f);

        // Volver al color según salud actual
        UpdateHealthColor();
    }

    void UpdateHealthUI()
    {
        if (healthBar != null)
        {
            healthBar.value = (float)currentHealth / maxHealth;
        }
    }

    void UpdateHealthColor()
    {
        if (!enableColorChange) return;

        // Calcular progreso del color (0 = salud llena, 1 = sin salud)
        float healthPercent = (float)currentHealth / maxHealth;
        Color targetColor = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);

        // Aplicar color a todos los renderers
        for (int i = 0; i < playerRenderers.Length; i++)
        {
            if (playerRenderers[i].material.HasProperty("_Color"))
            {
                playerRenderers[i].material.color = targetColor;
            }
        }
    }

    void Die()
    {
        Debug.Log("¡Jugador murió!");

        // Efecto de muerte
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Desactivar jugador temporalmente
        gameObject.SetActive(false);
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthUI();
        UpdateHealthColor(); // Actualizar color al curar
        Debug.Log($"Jugador curado. Vida: {currentHealth}");
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}