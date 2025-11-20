using UnityEngine;
using UnityEngine.AI;

public class CollectibleApple : MonoBehaviour
{
    [Header("Visual Effects")]
    public ParticleSystem glowParticles;
    public ParticleSystem trailParticles;
    public Light glowLight;
    public float rotationSpeed = 50f;
    public float floatHeight = 0.5f;
    public float floatSpeed = 2f;

    [Header("Enemy Activation")]
    public GameObject enemyToActivate;  // Cambiado: referencia al enemigo existente
    public bool activateEnemyOnPickup = true;  // Cambiado: activar en lugar de spawnear

    [Header("Audio")]
    public AudioClip pickupSound;

    private Vector3 startPosition;
    private bool isCollected = false;

    void Start()
    {
        startPosition = transform.position;

        // Iniciar efectos visuales
        StartVisualEffects();
    }

    void Update()
    {
        if (!isCollected)
        {
            // Animación de flotación y rotación
            FloatAndRotate();
        }
    }

    void StartVisualEffects()
    {
        // Activar partículas de glow
        if (glowParticles != null)
        {
            glowParticles.Play();
        }

        // Activar partículas de trail
        if (trailParticles != null)
        {
            trailParticles.Play();
        }

        // Activar luz
        if (glowLight != null)
        {
            glowLight.enabled = true;
        }

        // Si no hay efectos asignados, crear unos básicos
        if (glowParticles == null && glowLight == null)
        {
            CreateDefaultEffects();
        }
    }

    void CreateDefaultEffects()
    {
        // Crear partículas de glow automáticamente
        GameObject particlesObj = new GameObject("AppleGlowParticles");
        particlesObj.transform.SetParent(transform);
        particlesObj.transform.localPosition = Vector3.zero;

        glowParticles = particlesObj.AddComponent<ParticleSystem>();
        var main = glowParticles.main;
        var emission = glowParticles.emission;
        var shape = glowParticles.shape;

        main.startColor = Color.red;
        main.startSize = 0.1f;
        main.startLifetime = 1f;
        main.loop = true;

        emission.rateOverTime = 20f;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        glowParticles.Play();

        // Crear luz automáticamente
        GameObject lightObj = new GameObject("AppleGlowLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;

        glowLight = lightObj.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = Color.red;
        glowLight.range = 3f;
        glowLight.intensity = 2f;
    }

    void FloatAndRotate()
    {
        // Rotación continua
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Flotación suave
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isCollected && other.CompareTag("Player"))
        {
            CollectApple();
        }
    }

    void CollectApple()
    {
        isCollected = true;

        // Sonido de recogida
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }

        // Efectos visuales de recogida
        PlayPickupEffects();

        // Activar enemigo si está configurado
        if (activateEnemyOnPickup)
        {
            ActivateEnemy();
        }

        // Destruir la manzana después de un breve momento para que los efectos terminen
        Destroy(gameObject, 0.5f);
    }

    void PlayPickupEffects()
    {
        // Detener efectos normales
        if (glowParticles != null)
        {
            glowParticles.Stop();
        }

        if (trailParticles != null)
        {
            trailParticles.Stop();
        }

        if (glowLight != null)
        {
            glowLight.enabled = false;
        }
    }

    void ActivateEnemy()
    {
        if (enemyToActivate == null)
        {
            Debug.LogError("No hay enemigo asignado para activar.");
            return;
        }

        // Activar el enemigo
        enemyToActivate.SetActive(true);

        // Configurar componentes del enemigo
        SetupEnemy(enemyToActivate);

        Debug.Log("¡Enemigo activado!");
    }

    void SetupEnemy(GameObject enemy)
    {
        // Asegurarse de que el enemigo tenga los componentes necesarios
        if (enemy.GetComponent<Renderer>() == null)
        {
            Debug.LogWarning("El enemigo no tiene Renderer. No será visible.");
        }

        // Asegurarse de que el NavMeshAgent esté configurado
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent != null)
        {
            if (!agent.isOnNavMesh)
            {
                // Intentar reposicionar en NavMesh
                agent.Warp(enemy.transform.position);
            }
        }
        else
        {
            Debug.LogWarning("El enemigo no tiene NavMeshAgent. No podrá moverse.");
        }

        // Asegurarse de que tenga el script de seguimiento
        if (enemy.GetComponent<EnemyFollower>() == null)
        {
            Debug.LogWarning("El enemigo no tiene script EnemyFollower. No seguirá al jugador.");
        }

        // Asegurarse de que tenga el script de daño
        if (enemy.GetComponent<EnemyDamageFinal>() == null)
        {
            Debug.LogWarning("El enemigo no tiene script EnemyDamageFinal. No hará daño.");
        }
    }

    // Debug visual para ver el área de detección
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}