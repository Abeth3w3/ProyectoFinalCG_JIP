using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SimpleScoreUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI sceneText;

    [Header("Configuración")]
    public float updateInterval = 0.1f;

    private float lastUpdateTime;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        UpdateUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateUI();
    }

    void Update()
    {
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }

    void UpdateUI()
    {
        // Buscar GameDataManager directamente
        GameDataManager gameDataManager = GameDataManager.Instance;

        if (gameDataManager == null)
        {
            // Intentar buscar en la escena
            gameDataManager = FindAnyObjectByType<GameDataManager>();

            if (gameDataManager == null)
            {
                SetErrorState();
                return;
            }
        }

        var gameData = gameDataManager.GetGameData();
        if (gameData == null)
        {
            SetErrorState();
            return;
        }

        // Actualizar UI
        if (scoreText != null)
            scoreText.text = $"Puntos: {gameData.totalScore}";

        if (timerText != null)
            timerText.text = $"Tiempo: {gameData.currentLevelTime:F1}s";

        if (sceneText != null)
            sceneText.text = $"Escena: {SceneManager.GetActiveScene().name}";
    }

    void SetErrorState()
    {
        if (scoreText != null) scoreText.text = "Cargando...";
        if (timerText != null) timerText.text = "Sistema de puntos";
        if (sceneText != null) sceneText.text = $"Escena: {SceneManager.GetActiveScene().name}";
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}