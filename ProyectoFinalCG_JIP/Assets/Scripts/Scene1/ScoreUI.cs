using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;

    [Header("Configuración")]
    public float updateInterval = 0.1f;
    public Color normalTimeColor = Color.green;
    public Color warningTimeColor = Color.yellow;
    public Color criticalTimeColor = Color.red;

    private TimerScoreSystem scoreSystem;
    private float lastUpdateTime;

    void Start()
    {
        // Buscar el sistema de puntos en la escena
        scoreSystem = FindAnyObjectByType<TimerScoreSystem>();

        // Verificar que todo esté asignado
        if (scoreText == null)
        {
            Debug.LogError("ScoreText no asignado en ScoreUI");
        }

        if (timerText == null)
        {
            Debug.LogWarning("TimerText no asignado - el timer no se mostrará");
        }

        if (scoreSystem == null)
        {
            Debug.LogWarning("TimerScoreSystem no encontrado en la escena");
        }

        // Primera actualización
        UpdateUI();
    }

    void Update()
    {
        // Actualizar UI a intervalos regulares (mejor performance)
        if (Time.time - lastUpdateTime >= updateInterval)
        {
            UpdateUI();
            lastUpdateTime = Time.time;
        }
    }

    void UpdateUI()
    {
        // Actualizar texto de puntos
        if (scoreText != null && scoreSystem != null)
        {
            scoreText.text = $"Puntos: {scoreSystem.GetTotalScore()}";
        }
        else if (scoreText != null)
        {
            scoreText.text = "Puntos: 0";
        }

        // Actualizar texto del timer
        if (timerText != null && scoreSystem != null)
        {
            if (scoreSystem.IsEnemyAlive())
            {
                float currentTime = scoreSystem.GetCurrentTime();
                float maxTime = scoreSystem.GetMaxTime();

                // Actualizar texto
                timerText.text = $"Tiempo: {currentTime:F1}s / {maxTime}s";

                // Cambiar color según el tiempo
                if (currentTime <= maxTime)
                {
                    timerText.color = normalTimeColor; // Verde - tiempo bueno
                }
                else if (currentTime <= maxTime * 1.5f)
                {
                    timerText.color = warningTimeColor; // Amarillo - tiempo regular
                }
                else
                {
                    timerText.color = criticalTimeColor; // Rojo - tiempo malo
                }
            }
            else
            {
                timerText.text = "Buscando enemigo...";
                timerText.color = normalTimeColor;
            }
        }
    }

    // Método público para forzar actualización
    public void RefreshUI()
    {
        UpdateUI();
    }
}