using UnityEngine;
using UnityEngine.SceneManagement; // Para reiniciar el nivel

public class PlayerHealthFinal : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public int maxHealth = 100;
    public int currentHealth;
    public Renderer playerRenderer;

    [Header("Efecto de Muerte")]
    public GameObject gameOverUI; // Opcional: UI de Game Over
    public bool restartOnDeath = true;
    public float restartDelay = 3f;

    public bool isDead = false;
    private Color originalColor;
    private Material playerMaterial;

    void Start()
    {
        currentHealth = maxHealth;
        isDead = false;

        // Guardar el color original del material
        if (playerRenderer != null)
        {
            playerMaterial = playerRenderer.material;
            originalColor = playerMaterial.color;
            Debug.Log("Material asignado correctamente. Color original: " + originalColor);
        }
        else
        {
            Debug.LogError("PlayerRenderer no asignado en el Inspector!");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Daño recibido: " + damage + ". Vida restante: " + currentHealth);

        // Actualizar efecto visual
        UpdateHealthColor();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthColor()
    {
        if (playerMaterial == null)
        {
            Debug.LogWarning("Material no encontrado");
            return;
        }

        float healthPercent = (float)currentHealth / maxHealth;

        // Interpolar entre rojo (0% vida) y color original (100% vida)
        Color targetColor = Color.Lerp(Color.red, originalColor, healthPercent);
        playerMaterial.color = targetColor;

        Debug.Log("Color actualizado. Porcentaje de vida: " + healthPercent + ", Color: " + targetColor);
    }

    void Die()
    {
        isDead = true;
        Debug.Log("🎮 ¡JUGADOR MUERTO!");

        // Efecto visual al morir
        if (playerMaterial != null)
        {
            playerMaterial.color = Color.gray;
        }

        // Desactivar movimiento y controles
        DisablePlayerControls();

        // Mostrar Game Over UI si está asignado
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }

        // Reiniciar nivel después de un delay
        if (restartOnDeath)
        {
            Invoke("RestartLevel", restartDelay);
        }
    }

    void DisablePlayerControls()
    {
        // Desactivar scripts de movimiento
        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this && script.enabled)
            {
                script.enabled = false;
            }
        }

        // Desactivar Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Debug.Log("Controles desactivados");
    }

    void RestartLevel()
    {
        Debug.Log("Reiniciando nivel...");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Método para debugging en el Editor
    void OnGUI()
    {
        if (isDead)
        {
            GUI.color = Color.red;
            GUI.Label(new Rect(10, 10, 200, 30), "¡ESTÁS MUERTO!");
        }
        else
        {
            GUI.color = Color.green;
            GUI.Label(new Rect(10, 10, 200, 30), "Vida: " + currentHealth + "/" + maxHealth);
        }
    }
}