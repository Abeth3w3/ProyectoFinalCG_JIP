using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Lanzamiento de Burgers")]
    public Transform throwPoint;
    public GameObject burgerProjectile;
    public float throwForce = 15f;
    public float attackCooldown = 0.5f;

    [Header("Visual de Arma")]
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
        if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
        {
            ThrowBurger();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UseDrink();
        }
    }

    void ThrowBurger()
    {
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.slot1 != null &&
            InventoryManager.Instance.slot1.quantity > 0)
        {
            if (animator != null)
                animator.SetTrigger("Throw");

            if (burgerProjectile != null && throwPoint != null)
            {
                GameObject projectile = Instantiate(burgerProjectile, throwPoint.position, throwPoint.rotation);
                Rigidbody rb = projectile.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.AddForce(throwPoint.forward * throwForce, ForceMode.Impulse);
                }

                Debug.Log("Burger lanzada!");
            }

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.AddBurgerThrown();
            }

            InventoryManager.Instance.UseSlot1();
            attackTimer = attackCooldown;
        }
        else
        {
            Debug.Log("No hay burgers para lanzar");
        }
    }

    void UseDrink()
    {
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.slot2 != null &&
            InventoryManager.Instance.slot2.quantity > 0)
        {
            PlayerHealth playerHealth = GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(25);
            }

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.AddDrinkUsed();
            }

            InventoryManager.Instance.UseSlot2();
        }
    }

    void UpdateWeaponDisplay()
    {
        if (burgerInHand != null && InventoryManager.Instance != null)
        {
            bool hasBurgers = InventoryManager.Instance.slot1 != null &&
                            InventoryManager.Instance.slot1.quantity > 0;
            burgerInHand.SetActive(hasBurgers);
        }
    }
}