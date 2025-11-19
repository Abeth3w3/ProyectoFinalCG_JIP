using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Elements")]
    public GameObject gameOverPanel;
    public GameObject victoryPanel;
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI finalPointsText;
    public TextMeshProUGUI timeRemainingText;

    [Header("Game Settings")]
    public int basePoints = 100;
    public int mediumPoints = 70;
    public int lowPoints = 50;

    private bool gameEnded = false;
    private float gameTimeRemaining;
    private int playerPoints = 0;
    private GameTimer gameTimer;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameTimer = FindObjectOfType<GameTimer>();

        // Asegurarse de que los paneles estén desactivados al inicio
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (victoryPanel != null)
            victoryPanel.SetActive(false);
    }

    void Update()
    {
        // Actualizar tiempo restante para calcular puntos
        if (gameTimer != null && !gameEnded)
        {
            gameTimeRemaining = gameTimer.GetCurrentTime();

            if (timeRemainingText != null)
            {
                timeRemainingText.text = "Tiempo: " + Mathf.CeilToInt(gameTimeRemaining).ToString();
            }
        }
    }

    public void PlayerDied()
    {
        if (gameEnded) return;

        gameEnded = true;
        ShowGameOver();
        Debug.Log("Game Over - Jugador murió");
    }

    public void BossDefeated()
    {
        if (gameEnded) return;

        gameEnded = true;
        CalculatePoints();
        ShowVictory();
        Debug.Log("¡Victoria! Boss derrotado");
    }

    void CalculatePoints()
    {
        // Calcular puntos basados en el tiempo restante
        if (gameTimeRemaining >= 45f)
        {
            playerPoints = basePoints; // 100 puntos
        }
        else if (gameTimeRemaining >= 30f)
        {
            playerPoints = mediumPoints; // 70 puntos
        }
        else if (gameTimeRemaining >= 20f)
        {
            playerPoints = lowPoints; // 50 puntos
        }
        else
        {
            playerPoints = 0; // Menos de 20 segundos = 0 puntos
        }

        // Actualizar texto de puntos finales
        if (finalPointsText != null)
        {
            finalPointsText.text = "Puntos: " + playerPoints.ToString();
        }

        Debug.Log("Puntos obtenidos: " + playerPoints + " (Tiempo restante: " + Mathf.CeilToInt(gameTimeRemaining) + "s)");
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Pausar el juego
        Time.timeScale = 0f;

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void ShowVictory()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Pausar el juego
        Time.timeScale = 0f;

        // Mostrar cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Botones de UI
    public void RestartGame()
    {
        // Reanudar tiempo
        Time.timeScale = 1f;

        // Recargar la escena actual
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Application.Quit();

        // Para testing en el editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    // Para agregar puntos durante el juego (por ejemplo, por recoger manzanas)
    public void AddPoints(int points)
    {
        if (!gameEnded)
        {
            playerPoints += points;
            UpdatePointsUI();
        }
    }

    void UpdatePointsUI()
    {
        if (pointsText != null)
        {
            pointsText.text = "Puntos: " + playerPoints.ToString();
        }
    }
}