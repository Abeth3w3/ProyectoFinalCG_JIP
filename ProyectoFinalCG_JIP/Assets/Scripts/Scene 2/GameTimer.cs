using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public float timeElapsed = 0f;
    public int finalScore = 0;
    public bool isCounting = true;

    public TextMeshProUGUI scoreText;

    void Update()
    {
        if (isCounting)
        {
            timeElapsed += Time.deltaTime;
            scoreText.text = "Tiempo: " + timeElapsed.ToString("F1") + "  - Puntos: " + finalScore;
        }
    }

    public void StopAndCalculateScore()
    {
        isCounting = false;

        if (timeElapsed <= 10f)
            finalScore = 100;
        else if (timeElapsed <= 20f)
            finalScore = 70;
        else if (timeElapsed <= 30f)
            finalScore = 50;
        else
            finalScore = 20; // por si se demora más

        scoreText.text = "Tiempo: " + timeElapsed.ToString("F1") + "  - Puntos: " + finalScore;
    }
}
