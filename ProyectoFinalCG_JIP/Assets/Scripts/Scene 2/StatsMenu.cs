using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class StatsMenu : MonoBehaviour
{
    public TMP_Text tiempoJugadoText;
    public TMP_Text scoreText;

    private void Start()
    {
        var stats = PlayerStats.Load();
        tiempoJugadoText.text = $"Tiempo Jugado: {stats.tiempoJugado:F2} min";
        scoreText.text = $"Score: {stats.score}";
    }

    public void VolverAlMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
