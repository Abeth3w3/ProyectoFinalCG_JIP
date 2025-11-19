using UnityEngine;

public class OrbitalWeaponPickup : MonoBehaviour
{
    [Header("Orbital Settings")]
    public Transform playerCenter; // El transform del personaje

    [Header("Orbit Configuration")]
    public float orbitRadius = 1.5f;
    public float orbitSpeed = 200f;
    public float orbitHeight = 1.0f; // Altura desde el suelo del personaje

    [Header("Sword Rotation")]
    public Vector3 swordRotation = new Vector3(90, 0, 0); // Espada acostada

    [Header("Pickup Settings")]
    public float pickupRange = 2.5f;

    private GameObject currentWeapon;
    private bool hasWeapon = false;

    void Start()
    {
        // Si no se asignó playerCenter, usar el transform del personaje
        if (playerCenter == null)
        {
            playerCenter = transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !hasWeapon)
        {
            TryPickupWeapon();
        }

        if (hasWeapon && Input.GetKeyDown(KeyCode.G))
        {
            DropWeapon();
        }
    }

    void TryPickupWeapon()
    {
        Collider[] nearbyObjects = Physics.OverlapSphere(transform.position, pickupRange);

        foreach (Collider col in nearbyObjects)
        {
            if (col.CompareTag("Weapon"))
            {
                PickUpWeapon(col.gameObject);
                break;
            }
        }
    }

    void PickUpWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        hasWeapon = true;

        // Agregar y configurar componente orbital
        OrbitalSword orbital = currentWeapon.AddComponent<OrbitalSword>();
        orbital.SetOrbitCenter(playerCenter);

        // Configurar todos los parámetros
        orbital.orbitRadius = orbitRadius;
        orbital.orbitSpeed = orbitSpeed;
        orbital.SetOrbitHeight(orbitHeight);
        orbital.SetSwordRotation(swordRotation);

        // Desactivar física
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = currentWeapon.GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        Debug.Log($"Órbita activada - Altura: {orbitHeight}, Radio: {orbitRadius}");
    }

    void DropWeapon()
    {
        if (!hasWeapon) return;

        OrbitalSword orbital = currentWeapon.GetComponent<OrbitalSword>();
        if (orbital != null) Destroy(orbital);

        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        }

        Collider col = currentWeapon.GetComponent<Collider>();
        if (col != null) col.isTrigger = false;

        hasWeapon = false;
        currentWeapon = null;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        // Dibujar la órbita esperada
        Vector3 center = (playerCenter != null) ? playerCenter.position : transform.position;
        center += Vector3.up * orbitHeight;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(center, orbitRadius);
    }
}