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

    [Header("Respawn")]
    public Transform respawnPoint;
    public float respawnDelay = 1.5f;

    private bool isInvincible = false;
    private float invincibilityTimer = 0f;
    private PlayerMovement playerMovement;
    private CharacterController controller;

    void Start()
    {
        currentHealth = maxHealth;
        playerMovement = GetComponent<PlayerMovement>();
        controller = GetComponent<CharacterController>();
        UpdateHealthUI();
    }

    void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
                isInvincible = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible || (playerMovement != null && playerMovement.IsInvincible()))
            return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Jugador recibe {damage} daño. HP: {currentHealth}/{maxHealth}");

        if (damageSound != null)
            AudioSource.PlayClipAtPoint(damageSound, transform.position);

        if (damageParticles != null)
            Instantiate(damageParticles, transform.position, Quaternion.identity);

        isInvincible = true;
        invincibilityTimer = invincibilityTime;

        UpdateHealthUI();

        if (currentHealth <= 0)
            Die();
    }

    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"Curado {healAmount} HP. HP: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
    }

    void UpdateHealthUI()
    {
        if (healthText != null)
            healthText.text = $"HP: {currentHealth}/{maxHealth}";
    }

    void Die()
    {
        Debug.Log("☠ Jugador murió");

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (controller != null)
            controller.enabled = false;

        Invoke(nameof(Respawn), respawnDelay);
    }

    void Respawn()
    {
        if (respawnPoint == null)
        {
            Debug.LogError("NO ASIGNASTE EL RespawnPoint EN EL INSPECTOR!");
            return;
        }

        currentHealth = maxHealth;
        UpdateHealthUI();

        transform.position = respawnPoint.position;
        transform.rotation = respawnPoint.rotation;

        if (controller != null)
            controller.enabled = true;

        if (playerMovement != null)
            playerMovement.enabled = true;

        Debug.Log("🟢 Player respawneado con éxito");
    }
}
