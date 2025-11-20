using UnityEngine;

public class OrbitalWeaponPickup : MonoBehaviour
{
    [Header("Orbital Settings")]
    public Transform orbitCenter;  // Debe ser el transform del jugador

    [Header("Pickup Settings")]
    public float pickupRange = 2.5f;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.G;

    [Header("Weapon Reference")]
    public GameObject weaponObject; // Referencia directa a la espada

    private OrbitalSword orbitalSword;
    private bool hasWeapon = false;
    private Vector3 weaponOriginalPosition;
    private Quaternion weaponOriginalRotation;
    private Rigidbody weaponRigidbody;
    private Collider weaponCollider;

    void Start()
    {
        // Si no se asignó manualmente, buscar la espada automáticamente
        if (weaponObject == null)
        {
            weaponObject = GameObject.FindGameObjectWithTag("Weapon");
        }

        if (weaponObject != null)
        {
            // Guardar estado original de la espada
            weaponOriginalPosition = weaponObject.transform.position;
            weaponOriginalRotation = weaponObject.transform.rotation;
            weaponRigidbody = weaponObject.GetComponent<Rigidbody>();
            weaponCollider = weaponObject.GetComponent<Collider>();

            // Obtener o agregar el componente OrbitalSword
            orbitalSword = weaponObject.GetComponent<OrbitalSword>();
            if (orbitalSword == null)
            {
                orbitalSword = weaponObject.AddComponent<OrbitalSword>();
            }

            // Desactivar orbital al inicio
            orbitalSword.enabled = false;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con tag 'Weapon'");
        }

        // Si no se asignó orbitCenter, usar el transform del jugador
        if (orbitCenter == null)
        {
            orbitCenter = transform;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(pickupKey) && !hasWeapon)
        {
            TryPickupWeapon();
        }

        if (hasWeapon && Input.GetKeyDown(dropKey))
        {
            DropWeapon();
        }
    }

    void TryPickupWeapon()
    {
        if (weaponObject == null) return;

        // Verificar distancia
        float distance = Vector3.Distance(transform.position, weaponObject.transform.position);

        if (distance <= pickupRange)
        {
            PickUpWeapon();
        }
        else
        {
            Debug.Log("Demasiado lejos de la espada. Distancia: " + distance);
        }
    }

    void PickUpWeapon()
    {
        if (orbitalSword == null) return;

        hasWeapon = true;

        // Configurar el orbital
        orbitalSword.enabled = true;
        orbitalSword.SetOrbitCenter(orbitCenter);

        // Desactivar física
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = true;
            weaponRigidbody.useGravity = false;
        }

        // Hacer el collider trigger para que no interfiera
        if (weaponCollider != null)
        {
            weaponCollider.isTrigger = true;
        }

        Debug.Log("¡Espada orbital activada! Orbitando alrededor del jugador.");
    }

    void DropWeapon()
    {
        if (!hasWeapon || orbitalSword == null) return;

        // Desactivar orbital
        orbitalSword.enabled = false;
        orbitalSword.SetOrbitCenter(null);

        // Reactivar física
        if (weaponRigidbody != null)
        {
            weaponRigidbody.isKinematic = false;
            weaponRigidbody.useGravity = true;
            // Agregar fuerza de lanzamiento
            weaponRigidbody.AddForce(transform.forward * 5f + Vector3.up * 2f, ForceMode.Impulse);
        }

        // Restaurar collider
        if (weaponCollider != null)
        {
            weaponCollider.isTrigger = false;
        }

        hasWeapon = false;

        Debug.Log("Espada soltada");
    }

    // Método para forzar recoger arma (útil para testing)
    public void ForcePickup()
    {
        if (!hasWeapon)
        {
            PickUpWeapon();
        }
    }

    void OnDrawGizmosSelected()
    {
        // Rango de recogida
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        // Línea hacia la espada si existe
        if (weaponObject != null)
        {
            Gizmos.color = hasWeapon ? Color.green : Color.white;
            Gizmos.DrawLine(transform.position, weaponObject.transform.position);
        }
    }
}