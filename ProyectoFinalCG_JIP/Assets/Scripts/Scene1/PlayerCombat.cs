using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Lanzamiento de Burgers")] // ← NUEVA SECCIÓN
    public Transform throwPoint;
    public GameObject burgerProjectile;
    public float throwForce = 15f;
    public float attackCooldown = 0.5f;

    [Header("Visual de Arma")] // ← NUEVA SECCIÓN
    public GameObject burgerInHand;

    private float attackTimer = 0f;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        UpdateWeaponDisplay();
    }

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        HandleCombatInput();
        UpdateWeaponDisplay();
    }

    void HandleCombatInput()
    {
        // ← MODIFICADO: Click izquierdo para lanzar burger
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            ThrowBurger();
        }

        // E - Usar Drink (mantenido)
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseDrink();
        }
    }

    // ← NUEVO MÉTODO: Lanzamiento real de burger
    void ThrowBurger()
    {
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.slot1 != null &&
            InventoryManager.Instance.slot1.quantity > 0)
        {
            // Animación
            if (animator != null)
                animator.SetTrigger("Throw");

            // Instanciar projectile
            if (burgerProjectile != null && throwPoint != null)
            {
                GameObject projectile = Instantiate(burgerProjectile, throwPoint.position, throwPoint.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
                }

                Debug.Log("🍔 Burger lanzada!");
            }

            // Consumir burger del inventario
            InventoryManager.Instance.UseSlot1();
            attackTimer = attackCooldown;
        }
        else
        {
            Debug.Log("No hay burgers para lanzar");
        }
    }

    // ← MODIFICADO: Ahora cura al jugador
    void UseDrink()
    {
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.slot2 != null &&
            InventoryManager.Instance.slot2.quantity > 0)
        {
            // Curar al jugador
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(25); // Curar 25 puntos
            }
            else
            {
                Debug.LogWarning("PlayerHealth no encontrado en el jugador");
            }

            // Consumir drink
            InventoryManager.Instance.UseSlot2();
        }
    }

    // ← NUEVO MÉTODO: Mostrar/ocultar burger en mano
    void UpdateWeaponDisplay()
    {
        if (burgerInHand != null && InventoryManager.Instance != null)
        {
            // Mostrar burger en mano si hay en el inventario
            bool hasBurgers = InventoryManager.Instance.slot1 != null &&
                            InventoryManager.Instance.slot1.quantity > 0;
            burgerInHand.SetActive(hasBurgers);
        }
    }
}