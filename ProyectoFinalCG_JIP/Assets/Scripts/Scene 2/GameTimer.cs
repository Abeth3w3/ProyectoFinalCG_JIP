using UnityEngine;
using TMPro;
using System.Collections;

public class GameTimer : MonoBehaviour
{
    [Header("Timer Settings")]
    public TextMeshProUGUI timerText;
    public float gameTime = 60f;

    [Header("Tutorial Settings")]
    public float tutorialDuration = 5f;

    private float currentTime;
    private bool timerStarted = false;
    private bool gameActive = true;

    void Start()
    {
        currentTime = gameTime;
        UpdateTimerDisplay();

        // No iniciar el timer inmediatamente
        timerStarted = false;

        // Esperar el tiempo del tutorial y luego empezar
        StartCoroutine(StartAfterTutorial());
    }

    IEnumerator StartAfterTutorial()
    {
        Debug.Log("Esperando que termine el tutorial...");

        // Esperar exactamente el tiempo que dura tu mensaje tutorial
        yield return new WaitForSeconds(tutorialDuration);

        // Ahora iniciar el timer del juego
        StartTimer();
    }

    void Update()
    {
        if (timerStarted && gameActive)
        {
            currentTime -= Time.deltaTime;
            UpdateTimerDisplay();

            if (currentTime <= 0f)
            {
                currentTime = 0f;
                gameActive = false;
                timerStarted = false;
                GameOver();
            }
        }
    }

    void StartTimer()
    {
        timerStarted = true;
        Debug.Log("¡Timer del juego iniciado!");
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(currentTime);
            timerText.text = "Tiempo: " + seconds.ToString();
        }
    }

    void GameOver()
    {
        Debug.Log("¡Fin del juego por tiempo!");
    }

    // MÉTODO NUEVO - Para que GameManager pueda obtener el tiempo actual
    public float GetCurrentTime()
    {
        return currentTime;
    }

    // Para agregar tiempo extra
    public void AddTime(float extraTime)
    {
        if (gameActive)
        {
            currentTime += extraTime;
        }
    }
}