using UnityEngine;

public class CollectibleApple : MonoBehaviour
{
    [Header("Visual Effects")]
    public ParticleSystem glowParticles;
    public ParticleSystem trailParticles;
    public ParticleSystem sparkleParticles;
    public Light glowLight;
    public float rotationSpeed = 80f;
    public float floatHeight = 0.8f;
    public float floatSpeed = 3f;

    [Header("Enemy Activation")]
    public GameObject enemyToActivate; // Arrastra el enemigo desactivado de la escena aquí
    public bool activateEnemyOnPickup = true;
    public Vector3 enemyActivationOffset = new Vector3(0, 0.5f, 0);

    [Header("Audio")]
    public AudioClip pickupSound;
    public AudioClip ambientSound;

    private Vector3 startPosition;
    private bool isCollected = false;
    private Renderer appleRenderer;
    private Color originalColor;
    private AudioSource audioSource;

    void Start()
    {
        startPosition = transform.position;
        appleRenderer = GetComponent<Renderer>();

        if (appleRenderer != null)
        {
            originalColor = appleRenderer.material.color;
        }

        // Agregar AudioSource para sonido ambiente
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.spatialBlend = 1f;
        audioSource.maxDistance = 15f;

        if (ambientSound != null)
        {
            audioSource.clip = ambientSound;
            audioSource.Play();
        }

        // Iniciar efectos visuales
        StartVisualEffects();

        // Verificar que el enemigo esté asignado
        if (activateEnemyOnPickup && enemyToActivate == null)
        {
            Debug.LogWarning("CollectibleApple: EnemyToActivate no está asignado pero activateEnemyOnPickup está activado.", this);
        }
        else if (enemyToActivate != null)
        {
            Debug.Log($"Enemigo asignado: {enemyToActivate.name}. Estado actual: {enemyToActivate.activeInHierarchy}");
        }
    }

    void Update()
    {
        if (!isCollected)
        {
            // Animación de flotación y rotación mejorada
            EnhancedFloatAndRotate();

            // Efecto de pulso en el color
            PulseColorEffect();

            // Efecto de pulso en la luz
            PulseLightEffect();
        }
    }

    void EnhancedFloatAndRotate()
    {
        // Rotación más rápida en múltiples ejes
        transform.Rotate(rotationSpeed * Time.deltaTime,
                        rotationSpeed * 0.7f * Time.deltaTime,
                        rotationSpeed * 0.3f * Time.deltaTime);

        // Flotación más exagerada con movimiento circular
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        float newX = startPosition.x + Mathf.Cos(Time.time * floatSpeed * 0.5f) * 0.3f;
        float newZ = startPosition.z + Mathf.Sin(Time.time * floatSpeed * 0.3f) * 0.2f;

        transform.position = new Vector3(newX, newY, newZ);
    }

    void PulseColorEffect()
    {
        if (appleRenderer != null)
        {
            // Pulso entre rojo normal y rojo brillante
            float pulse = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f;
            Color pulseColor = Color.Lerp(originalColor, Color.red * 1.5f, pulse);
            appleRenderer.material.color = pulseColor;
        }
    }

    void PulseLightEffect()
    {
        if (glowLight != null)
        {
            // La luz pulsa más intensamente
            float pulse = (Mathf.Sin(Time.time * 3f) + 1f) * 0.5f;
            glowLight.intensity = 2f + pulse * 3f;
            glowLight.range = 5f + pulse * 3f;
        }
    }

    void StartVisualEffects()
    {
        // Activar partículas de glow con más intensidad
        if (glowParticles != null)
        {
            var main = glowParticles.main;
            main.startSize = 0.2f;
            var emission = glowParticles.emission;
            emission.rateOverTime = 30f;
            glowParticles.Play();
        }

        // Activar partículas de trail
        if (trailParticles != null)
        {
            trailParticles.Play();
        }

        // Crear partículas de chispas si no existen
        if (sparkleParticles == null)
        {
            CreateSparkleParticles();
        }
        else
        {
            sparkleParticles.Play();
        }

        // Configurar luz más intensa
        if (glowLight != null)
        {
            glowLight.color = Color.red;
            glowLight.range = 8f;
            glowLight.intensity = 3f;
            glowLight.enabled = true;
        }
        else
        {
            CreateEnhancedLight();
        }

        // Si no hay efectos asignados, crear unos mejorados
        if (glowParticles == null)
        {
            CreateEnhancedParticles();
        }
    }

    void CreateSparkleParticles()
    {
        GameObject sparklesObj = new GameObject("SparkleParticles");
        sparklesObj.transform.SetParent(transform);
        sparklesObj.transform.localPosition = Vector3.zero;

        sparkleParticles = sparklesObj.AddComponent<ParticleSystem>();
        var main = sparkleParticles.main;
        var emission = sparkleParticles.emission;
        var shape = sparkleParticles.shape;

        main.startColor = new ParticleSystem.MinMaxGradient(Color.yellow, Color.red);
        main.startSize = 0.1f;
        main.startLifetime = 0.5f;
        main.loop = true;
        main.maxParticles = 50;

        emission.rateOverTime = 15f;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1.5f;

        sparkleParticles.Play();
    }

    void CreateEnhancedLight()
    {
        GameObject lightObj = new GameObject("EnhancedGlowLight");
        lightObj.transform.SetParent(transform);
        lightObj.transform.localPosition = Vector3.zero;

        glowLight = lightObj.AddComponent<Light>();
        glowLight.type = LightType.Point;
        glowLight.color = Color.red;
        glowLight.range = 10f;
        glowLight.intensity = 4f;
        glowLight.shadows = LightShadows.Soft;
    }

    void CreateEnhancedParticles()
    {
        // Partículas de glow mejoradas
        GameObject particlesObj = new GameObject("EnhancedGlowParticles");
        particlesObj.transform.SetParent(transform);
        particlesObj.transform.localPosition = Vector3.zero;

        glowParticles = particlesObj.AddComponent<ParticleSystem>();
        var main = glowParticles.main;
        var emission = glowParticles.emission;
        var shape = glowParticles.shape;
        var colorOverLifetime = glowParticles.colorOverLifetime;

        main.startColor = new ParticleSystem.MinMaxGradient(Color.red, Color.yellow);
        main.startSize = 0.3f;
        main.startLifetime = 2f;
        main.loop = true;
        main.maxParticles = 100;

        emission.rateOverTime = 40f;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 2f;

        // Gradiente de color durante la vida de la partícula
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.yellow, 0.0f),
                new GradientColorKey(Color.red, 0.5f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.0f, 0.0f),
                new GradientAlphaKey(1.0f, 0.1f),
                new GradientAlphaKey(0.8f, 0.5f),
                new GradientAlphaKey(0.0f, 1.0f)
            }
        );
        colorOverLifetime.color = gradient;

        glowParticles.Play();
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

        // Detener sonido ambiente
        if (audioSource != null)
        {
            audioSource.Stop();
        }

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
        // Explosión final de partículas
        if (glowParticles != null)
        {
            glowParticles.Stop();
            var emission = glowParticles.emission;
            emission.enabled = false;
        }

        if (sparkleParticles != null)
        {
            sparkleParticles.Stop();
        }

        if (trailParticles != null)
        {
            trailParticles.Stop();
        }

        if (glowLight != null)
        {
            glowLight.enabled = false;
        }

        // Crear una explosión final de partículas
        CreateFinalExplosion();
    }

    void CreateFinalExplosion()
    {
        GameObject explosionObj = new GameObject("FinalExplosion");
        explosionObj.transform.position = transform.position;

        ParticleSystem explosion = explosionObj.AddComponent<ParticleSystem>();
        var main = explosion.main;
        var emission = explosion.emission;
        var shape = explosion.shape;

        main.startColor = Color.white;
        main.startSize = 0.5f;
        main.startLifetime = 1f;
        main.loop = false;

        emission.burstCount = 1;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0f, 30) });

        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 1f;

        explosion.Play();
        Destroy(explosionObj, 2f);
    }

    void ActivateEnemy()
    {
        if (enemyToActivate != null)
        {
            // Mover el enemigo a la posición de la manzana con offset
            Vector3 activationPosition = transform.position + enemyActivationOffset;
            enemyToActivate.transform.position = activationPosition;

            // Activar el enemigo
            enemyToActivate.SetActive(true);

            Debug.Log($"¡Enemigo {enemyToActivate.name} activado en la posición de la manzana!");

            // Opcional: Reiniciar componentes del enemigo si es necesario
            ResetEnemyComponents();
        }
        else
        {
            Debug.LogWarning("CollectibleApple: No se pudo activar el enemigo porque enemyToActivate es null.");
        }
    }

    void ResetEnemyComponents()
    {
        // Si el enemigo tiene un NavMeshAgent, reiniciar su destino
        UnityEngine.AI.NavMeshAgent agent = enemyToActivate.GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = false;
        }

        // Si el enemigo tiene un componente de salud, podrías resetearlo si es necesario
        EnemyHealth enemyHealth = enemyToActivate.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            // Aquí podrías resetear la salud si lo deseas
            // enemyHealth.ResetHealth();
        }

        // Si el enemigo tiene un componente de IA, podrías reiniciar su estado
        SimpleEnemyAI enemyAI = enemyToActivate.GetComponent<SimpleEnemyAI>();
        if (enemyAI != null)
        {
            // El enemigo comenzará a perseguir automáticamente en su próximo Update
        }
    }

    // Debug visual para ver el área de detección
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.8f);

        // También dibujar la posición donde se activará el enemigo
        if (enemyToActivate != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position + enemyActivationOffset, 0.5f);
            Gizmos.DrawLine(transform.position, transform.position + enemyActivationOffset);
        }
    }
}