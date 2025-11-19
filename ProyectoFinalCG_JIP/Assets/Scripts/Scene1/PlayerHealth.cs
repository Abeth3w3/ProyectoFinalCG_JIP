using UnityEngine;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 100;
    public int currentHealth;
    public TextMeshProUGUI healthText;

    [Header("Damage Effects")]
    public AudioClip damageSound;
    public ParticleSystem damageParticles;
    public float invincibilityTime = 1f;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private PlayerMovement playerMovement;

    void Start()
    {
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        // Verificar invencibilidad (roll o daño reciente)
        if (isInvincible || (playerMovement != null && playerMovement.IsInvincible()))
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"💔 Jugador recibe {damage} daño. HP: {currentHealth}/{maxHealth}");

        // Efectos
        if (damageSound != null)
            AudioSource.PlayClipAtPoint(damageSound, transform.position);

        if (damageParticles != null)
            Instantiate(damageParticles, transform.position, Quaternion.identity);

        // Invencibilidad temporal
        isInvincible = true;
        invincibilityTimer = invincibilityTime;

        UpdateHealthUI();

        // Muerte
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"❤️ Curado {healAmount} HP. HP: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
        }
    }

    void Die()
    {
        Debug.Log("💀 Jugador murió");
        // Aquí puedes agregar respawn o game over
        Time.timeScale = 0; // Pausar juego
    }
}