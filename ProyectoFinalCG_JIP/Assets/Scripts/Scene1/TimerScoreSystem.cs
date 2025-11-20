using UnityEngine;

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
    private int totalScore = 0;

    void Start()
    {
        FindEnemy();
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
            totalScore += points;

            Debug.Log($"Enemigo derrotado en {killTime:F1}s! +{points} puntos | Total: {totalScore}");

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

    public void SetCurrentEnemy(GluttonyEnemy enemy)
    {
        if (enemy != null && enemy.currentHealth > 0)
        {
            StartTrackingEnemy(enemy);
        }
    }

    public int GetTotalScore()
    {
        return totalScore;
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

    public int GetCurrentPoints()
    {
        if (enemyAlive)
        {
            float currentTime = GetCurrentTime();
            return CalculatePoints(currentTime);
        }
        return maxPoints;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Puntos: {totalScore}");
        if (enemyAlive)
        {
            float timeElapsed = Time.time - enemySpawnTime;
            GUI.Label(new Rect(10, 30, 300, 20), $"Tiempo: {timeElapsed:F1}s / {maxTime}s");
        }
    }
}