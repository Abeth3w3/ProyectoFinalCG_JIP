using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsController : MonoBehaviour
{
    [Header("Canvas Principal")]
    public GameObject mainCanvas;

    [Header("Paneles de Controles")]
    public GameObject level1Panel;
    public GameObject level2Panel;
    public GameObject level3Panel;

    void Start()
    {
        ShowMainCanvas();
    }

    public void ShowMainCanvas()
    {
        // Mostrar canvas principal
        if (mainCanvas != null) mainCanvas.SetActive(true);

        // Ocultar todos los paneles
        DeactivateAllPanels();
    }

    private void DeactivateAllPanels()
    {
        if (level1Panel != null) level1Panel.SetActive(false);
        if (level2Panel != null) level2Panel.SetActive(false);
        if (level3Panel != null) level3Panel.SetActive(false);
    }

    public void ShowLevel1Controls()
    {
        // Ocultar canvas principal
        if (mainCanvas != null) mainCanvas.SetActive(false);

        // Mostrar solo el panel del nivel 1
        DeactivateAllPanels();
        if (level1Panel != null) level1Panel.SetActive(true);
    }

    public void ShowLevel2Controls()
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
        DeactivateAllPanels();
        if (level2Panel != null) level2Panel.SetActive(true);
    }

    public void ShowLevel3Controls()
    {
        if (mainCanvas != null) mainCanvas.SetActive(false);
        DeactivateAllPanels();
        if (level3Panel != null) level3Panel.SetActive(true);
    }

    // Este método lo llamará cada botón "Volver" en los paneles
    public void BackToLevelSelection()
    {
        ShowMainCanvas();
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}