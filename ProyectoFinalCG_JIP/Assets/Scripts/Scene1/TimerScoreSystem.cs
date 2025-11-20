using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScoreSystem : MonoBehaviour
{
    [Header("Configuración de Puntos")]
    public int maxPoints = 100;
    public int minPoints = 20;
    public float maxTime = 30f;

    [Header("Referencia del Enemigo")]
    public GluttonyEnemy currentEnemy;

    private float enemySpawnTime;
    private bool enemyAlive = false;

    void Awake()
    {
        // Hacer persistente entre escenas
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindEnemy();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reiniciar para nueva escena
        currentEnemy = null;
        enemyAlive = false;
        Invoke(nameof(FindEnemy), 0.5f);
    }

    void Update()
    {
        if (!enemyAlive || currentEnemy == null)
        {
            if (Time.frameCount % 120 == 0)
            {
                FindEnemy();
            }
        }
    }

    void FindEnemy()
    {
        if (enemyAlive && currentEnemy != null && currentEnemy.currentHealth > 0) return;

        GluttonyEnemy[] enemies = FindObjectsByType<GluttonyEnemy>(FindObjectsSortMode.None);

        foreach (GluttonyEnemy enemy in enemies)
        {
            if (enemy != null && enemy.currentHealth > 0 && enemy != currentEnemy)
            {
                StartTrackingEnemy(enemy);
                break;
            }
        }
    }

    void StartTrackingEnemy(GluttonyEnemy enemy)
    {
        if (currentEnemy != null)
        {
            currentEnemy.onEnemyDeath -= OnEnemyDeath;
        }

        currentEnemy = enemy;
        enemySpawnTime = Time.time;
        enemyAlive = true;

        enemy.onEnemyDeath += OnEnemyDeath;

        Debug.Log($"Comenzando timer para enemigo: {enemy.name}");
    }

    void OnEnemyDeath(GluttonyEnemy enemy)
    {
        if (enemy == currentEnemy && enemyAlive)
        {
            float killTime = Time.time - enemySpawnTime;
            int points = CalculatePoints(killTime);

            if (GameDataManager.Instance != null)
            {
                GameDataManager.Instance.AddScore(points);
                GameDataManager.Instance.AddEnemyKill();
            }

            Debug.Log($"Enemigo derrotado en {killTime:F1}s! +{points} puntos | Total: {GetTotalScore()}");

            enemyAlive = false;
            enemy.onEnemyDeath -= OnEnemyDeath;
            Invoke(nameof(FindEnemy), 3f);
        }
    }

    int CalculatePoints(float timeElapsed)
    {
        if (timeElapsed <= maxTime)
        {
            return maxPoints;
        }

        float timeOverMax = timeElapsed - maxTime;
        float reductionIntervals = timeOverMax / maxTime;
        int pointsReduction = Mathf.FloorToInt(reductionIntervals * (maxPoints - minPoints));

        int finalPoints = maxPoints - pointsReduction;
        return Mathf.Max(minPoints, finalPoints);
    }

    public int GetTotalScore()
    {
        if (GameDataManager.Instance != null)
        {
            return GameDataManager.Instance.GetGameData().totalScore;
        }
        return 0;
    }

    public bool IsEnemyAlive()
    {
        return enemyAlive && currentEnemy != null;
    }

    public float GetCurrentTime()
    {
        if (enemyAlive)
        {
            return Time.time - enemySpawnTime;
        }
        return 0f;
    }

    public float GetMaxTime()
    {
        return maxTime;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}