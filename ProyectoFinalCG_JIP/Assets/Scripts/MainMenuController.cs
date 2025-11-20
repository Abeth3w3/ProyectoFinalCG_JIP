using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Scene 1");
    }

    public void OpenStats()
    {
        SceneManager.LoadScene("StatsScene");
    }

    public void OpenControls()
    {
        SceneManager.LoadScene("ControlsScene");
    }

    public void ExitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}