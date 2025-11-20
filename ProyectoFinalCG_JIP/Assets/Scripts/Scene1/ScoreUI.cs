using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI sceneText;

    [Header("Configuración")]
    public float updateInterval = 0.1f;
    public Color normalTimeColor = Color.green;
    public Color warningTimeColor = Color.yellow;
    public Color criticalTimeColor = Color.red;

    private TimerScoreSystem scoreSystem;
    private float lastUpdateTime;

    void Awake()
    {
        // Hacer persistente entre escenas
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        FindScoreSystem();
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (scoreText == null) Debug.LogError("ScoreText no asignado");
        if (timerText == null) Debug.LogWarning("TimerText no asignado");

        UpdateUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Actualizar texto de escena
        if (sceneText != null)
        {
            sceneText.text = $"Escena: {scene.name}";
        }

        // Buscar el sistema de puntuación en la nueva escena
        FindScoreSystem();
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

    void FindScoreSystem()
    {
        // Reemplazar FindObjectOfType con la versión no obsoleta
        scoreSystem = FindFirstObjectByType<TimerScoreSystem>();

        // Si no se encuentra, intentar con la versión más rápida
        if (scoreSystem == null)
        {
            scoreSystem = FindAnyObjectByType<TimerScoreSystem>();
        }
    }

    void UpdateUI()
    {
        if (scoreSystem == null)
        {
            FindScoreSystem();
            return;
        }

        // Actualizar puntuación
        if (scoreText != null)
        {
            scoreText.text = $"Puntos: {scoreSystem.GetTotalScore()}";
        }

        // Actualizar timer
        if (timerText != null)
        {
            if (scoreSystem.IsEnemyAlive())
            {
                float currentTime = scoreSystem.GetCurrentTime();
                float maxTime = scoreSystem.GetMaxTime();

                timerText.text = $"Tiempo: {currentTime:F1}s / {maxTime}s";

                // Cambiar color según tiempo
                if (currentTime <= maxTime)
                {
                    timerText.color = normalTimeColor;
                }
                else if (currentTime <= maxTime * 1.5f)
                {
                    timerText.color = warningTimeColor;
                }
                else
                {
                    timerText.color = criticalTimeColor;
                }
            }
            else
            {
                timerText.text = "Buscando enemigo...";
                timerText.color = normalTimeColor;
            }
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}