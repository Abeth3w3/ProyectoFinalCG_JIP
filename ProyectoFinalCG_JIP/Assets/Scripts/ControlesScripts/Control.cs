using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlsController : MonoBehaviour
{
    [Header("Vistas Principales")]
    public GameObject levelSelectionView; // Vista con todos los botones de niveles
    public GameObject controlsView; // Vista padre para cuando se muestra cualquier panel de controles

    [Header("Paneles de Controles")]
    public GameObject level1Panel;
    public GameObject level2Panel;
    public GameObject level3Panel;

    [Header("Botón Atrás en Vista de Controles")]
    public GameObject backButton; // Botón "Atrás" que solo aparece cuando se ve un panel

    void Start()
    {
        // Al inicio: mostrar selección de niveles, ocultar todo lo demás
        ShowLevelSelection();
    }

    // Mostrar la vista de selección de niveles
    public void ShowLevelSelection()
    {
        // Mostrar selección de niveles
        if (levelSelectionView != null) levelSelectionView.SetActive(true);

        // Ocultar vista de controles
        if (controlsView != null) controlsView.SetActive(false);

        // Ocultar botón atrás
        if (backButton != null) backButton.SetActive(false);

        // Asegurar que todos los paneles estén desactivados
        DeactivateAllPanels();
    }

    // Método para desactivar todos los paneles de controles
    private void DeactivateAllPanels()
    {
        if (level1Panel != null) level1Panel.SetActive(false);
        if (level2Panel != null) level2Panel.SetActive(false);
        if (level3Panel != null) level3Panel.SetActive(false);
    }

    public void ShowLevel1Controls()
    {
        // Ocultar selección de niveles
        if (levelSelectionView != null) levelSelectionView.SetActive(false);

        // Mostrar vista de controles
        if (controlsView != null) controlsView.SetActive(true);

        // Mostrar botón atrás
        if (backButton != null) backButton.SetActive(true);

        // Activar solo el panel del nivel 1
        DeactivateAllPanels();
        if (level1Panel != null) level1Panel.SetActive(true);
    }

    public void ShowLevel2Controls()
    {
        if (levelSelectionView != null) levelSelectionView.SetActive(false);
        if (controlsView != null) controlsView.SetActive(true);
        if (backButton != null) backButton.SetActive(true);

        DeactivateAllPanels();
        if (level2Panel != null) level2Panel.SetActive(true);
    }

    public void ShowLevel3Controls()
    {
        if (levelSelectionView != null) levelSelectionView.SetActive(false);
        if (controlsView != null) controlsView.SetActive(true);
        if (backButton != null) backButton.SetActive(true);

        DeactivateAllPanels();
        if (level3Panel != null) level3Panel.SetActive(true);
    }

    // Método para el botón "Atrás" - regresa a selección de niveles
    public void BackToLevelSelection()
    {
        ShowLevelSelection();
    }

    // Método para el botón "Volver al Menú Principal"
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}