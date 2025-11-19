using UnityEngine;

public class CollectableItem : MonoBehaviour
{
    [Header("Configuración del Item")]
    public string itemName = "burger"; // "burger" o "drink"
    public int amount = 1;
    public AudioClip collectSound;
    public ParticleSystem collectParticles;

    [Header("Animación")]
    public float rotationSpeed = 50f;
    public float floatAmplitude = 0.5f;
    public float floatFrequency = 1f;

    private Vector3 startPos;
    private bool isCollected = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (isCollected) return;

        // Animación flotante
        float newY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
        transform.position = startPos + new Vector3(0, newY, 0);
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            Collect();
        }
    }

    void Collect()
    {
        isCollected = true;

        // Agregar al sistema de slots
        InventoryManager.Instance.AddItem(itemName, amount);

        // Efectos
        if (collectSound != null)
            AudioSource.PlayClipAtPoint(collectSound, transform.position);

        if (collectParticles != null)
        {
            var particles = Instantiate(collectParticles, transform.position, Quaternion.identity);
            Destroy(particles.gameObject, 2f);
        }

        Destroy(gameObject);
    }
}