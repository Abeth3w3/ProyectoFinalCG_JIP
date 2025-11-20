using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    private GameData gameData;
    private string savePath;
    private float levelStartTime;
    private string currentLevelName;

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
        if (gameData != null && !string.IsNullOrEmpty(currentLevelName))
        {
            SaveLevelTime(currentLevelName, Time.time - levelStartTime);
        }

        levelStartTime = Time.time;
        currentLevelName = scene.name;
        gameData.currentLevel = currentLevelName;
    }

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

    public void CompleteLevel()
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