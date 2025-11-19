using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject weaponHandle;
    public Animator animator;

    [Header("Weapon Position Adjustments")]
    public Vector3 positionOffset = Vector3.zero;
    public Vector3 rotationOffset = Vector3.zero;

    [Header("Pickup Settings")]
    public float pickupRange = 2f;
    public KeyCode pickupKey = KeyCode.E;

    private GameObject currentWeapon;
    private bool hasWeapon = false;
    private GameObject weaponInRange;

    void Update()
    {
        CheckForWeapons();

        // Recoger arma
        if (Input.GetKeyDown(pickupKey) && weaponInRange != null && !hasWeapon)
        {
            PickUpWeapon(weaponInRange);
        }

        // Atacar si tiene arma
        if (hasWeapon && Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("Attack");
        }
    }

    void CheckForWeapons()
    {
        // Buscar armas cercanas
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, pickupRange);
        GameObject closestWeapon = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Weapon"))
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestWeapon = hitCollider.gameObject;
                }
            }
        }

        weaponInRange = closestWeapon;

        // Mostrar UI de recogida si hay arma cerca
        if (weaponInRange != null && !hasWeapon)
        {
            Debug.Log("Presiona E para recoger el arma");
            // Aquí puedes mostrar un texto en UI: "Presiona E para recoger"
        }
    }

    void PickUpWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        hasWeapon = true;

        // Desactivar física
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        Collider col = weapon.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        if (col != null)
        {
            col.enabled = false;
        }

        // Parentear al punto de agarre
        weapon.transform.SetParent(weaponHandle.transform);
        weapon.transform.localPosition = positionOffset;
        weapon.transform.localRotation = Quaternion.Euler(rotationOffset);

        Debug.Log("Arma recogida: " + weapon.name);

        // Opcional: Reproducir sonido de recogida
        // AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }

    // Método para soltar el arma (opcional)
    public void DropWeapon()
    {
        if (!hasWeapon) return;

        // Quitar parent
        currentWeapon.transform.SetParent(null);

        // Reactivar física
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        Collider col = currentWeapon.GetComponent<Collider>();

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        if (col != null)
        {
            col.enabled = true;
        }

        // Lanzar el arma ligeramente hacia adelante
        if (rb != null)
        {
            rb.AddForce(transform.forward * 5f, ForceMode.Impulse);
        }

        hasWeapon = false;
        currentWeapon = null;
    }

    // Visualizar rango de recogida en el editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}