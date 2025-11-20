using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Level System Configuration")]
    public string[] levelNames = { "Scene1", "Scene2", "Scene3" };
    public float levelTransitionDelay = 3f;

    private GameData gameData;
    private string savePath;
    private float levelStartTime;
    private string currentLevelName;
    private int currentLevelIndex = 0;
    private bool levelCompleted = false;

    // Lista de enemigos en la escena actual
    private List<GameObject> currentEnemies = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        levelStartTime = Time.time;
        currentLevelName = SceneManager.GetActiveScene().name;
    }

    void Update()
    {
        if (gameData != null)
        {
            gameData.totalPlayTime += Time.deltaTime;
            gameData.currentLevelTime = Time.time - levelStartTime;
        }
    }

    void InitializeSaveSystem()
    {
        savePath = Path.Combine(Application.persistentDataPath, "gameSave.json");
        LoadGame();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Guardar tiempo del nivel anterior
        if (gameData != null && !string.IsNullOrEmpty(currentLevelName) && currentLevelName != scene.name)
        {
            SaveLevelTime(currentLevelName, Time.time - levelStartTime);
        }

        // Reiniciar para nuevo nivel
        levelStartTime = Time.time;
        currentLevelName = scene.name;
        gameData.currentLevel = currentLevelName;

        // Reiniciar estado del nivel
        levelCompleted = false;
        currentEnemies.Clear();

        // Buscar y registrar todos los enemigos en la escena
        RegisterAllEnemiesInScene();

        Debug.Log($"🎮 Nivel '{currentLevelName}' iniciado. Enemigos: {currentEnemies.Count}");
    }

    void RegisterAllEnemiesInScene()
    {
        // Buscar enemigos por tag
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemyObjects)
        {
            RegisterEnemy(enemy);
        }

        // También podemos buscar por componentes específicos por si acaso
        GluttonyEnemy[] gluttonyEnemies = FindObjectsOfType<GluttonyEnemy>();
        foreach (GluttonyEnemy enemy in gluttonyEnemies)
        {
            if (!currentEnemies.Contains(enemy.gameObject))
            {
                RegisterEnemy(enemy.gameObject);
            }
        }

        SimpleEnemy[] simpleEnemies = FindObjectsOfType<SimpleEnemy>();
        foreach (SimpleEnemy enemy in simpleEnemies)
        {
            if (!currentEnemies.Contains(enemy.gameObject))
            {
                RegisterEnemy(enemy.gameObject);
            }
        }
    }

    public void RegisterEnemy(GameObject enemy)
    {
        if (!currentEnemies.Contains(enemy))
        {
            currentEnemies.Add(enemy);
        }
    }

    public void OnEnemyKilled(GameObject enemy, int points = 100)
    {
        // Remover el enemigo de la lista
        if (currentEnemies.Contains(enemy))
        {
            currentEnemies.Remove(enemy);
        }

        // Actualizar datos
        AddEnemyKill();
        AddScore(points);

        Debug.Log($"⚔️ Enemigo eliminado! Puntos: {points} | Enemigos restantes: {currentEnemies.Count}");

        // Verificar si no quedan enemigos
        if (currentEnemies.Count == 0 && !levelCompleted)
        {
            CompleteLevel();
        }
    }

    void CompleteLevel()
    {
        levelCompleted = true;
        float levelTime = Time.time - levelStartTime;

        Debug.Log($"✅ ¡Nivel {currentLevelName} completado! Tiempo: {levelTime:F1}s");

        // Bonus por completar nivel
        AddScore(500); // Bonus fijo por completar el nivel
        CompleteLevelRecord();

        // Cambiar al siguiente nivel después del delay
        Invoke("LoadNextLevel", levelTransitionDelay);
    }

    void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelNames.Length)
        {
            string nextLevel = levelNames[currentLevelIndex];
            Debug.Log($"🔄 Cargando siguiente nivel: {nextLevel}");
            SceneManager.LoadScene(nextLevel);
        }
        else
        {
            Debug.Log("🎊 ¡JUEGO COMPLETADO! Todos los niveles terminados.");
            // Aquí puedes cargar una escena de final de juego
        }
    }

    // ... (el resto de métodos de guardado y carga se mantienen igual)

    public void SaveGame()
    {
        try
        {
            gameData.lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string jsonData = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(savePath, jsonData);
            Debug.Log("Juego guardado: " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogError("Error guardando: " + e.Message);
        }
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string jsonData = File.ReadAllText(savePath);
                gameData = JsonUtility.FromJson<GameData>(jsonData);

                if (gameData.levelTimes == null)
                    gameData.levelTimes = new List<LevelTime>();
            }
            catch (Exception e)
            {
                Debug.LogError("Error cargando: " + e.Message);
                CreateNewGameData();
            }
        }
        else
        {
            CreateNewGameData();
        }
    }

    void CreateNewGameData()
    {
        gameData = new GameData()
        {
            totalScore = 0,
            highScore = 0,
            totalDeaths = 0,
            enemiesKilled = 0,
            totalPlayTime = 0,
            currentLevelTime = 0,
            levelTimes = new List<LevelTime>(),
            burgersThrown = 0,
            drinksUsed = 0,
            levelsCompleted = 0,
            burgerCount = 10,
            drinkCount = 5,
            coins = 100,
            musicVolume = 0.8f,
            sfxVolume = 0.8f,
            tutorialCompleted = false,
            lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            currentLevel = SceneManager.GetActiveScene().name
        };
    }

    void SaveLevelTime(string levelName, float completionTime)
    {
        var levelTime = new LevelTime()
        {
            levelName = levelName,
            completionTime = completionTime,
            dateCompleted = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
        };

        gameData.levelTimes.Add(levelTime);
    }

    public void AddScore(int points)
    {
        gameData.totalScore += points;
        gameData.highScore = Mathf.Max(gameData.highScore, gameData.totalScore);
        SaveGame();
    }

    public void AddEnemyKill()
    {
        gameData.enemiesKilled++;
        SaveGame();
    }

    public void AddPlayerDeath()
    {
        gameData.totalDeaths++;
        SaveGame();
    }

    public void AddBurgerThrown()
    {
        gameData.burgersThrown++;
        SaveGame();
    }

    public void AddDrinkUsed()
    {
        gameData.drinksUsed++;
        SaveGame();
    }

    public void CompleteLevelRecord()
    {
        gameData.levelsCompleted++;
        SaveGame();
    }

    public GameData GetGameData()
    {
        return gameData;
    }

    void OnApplicationQuit()
    {
        SaveGame();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}