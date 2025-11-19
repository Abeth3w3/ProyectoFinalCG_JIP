using UnityEngine;
using TMPro;
using System.Collections;

public class TutorialMessage : MonoBehaviour
{
    [Header("Configuración del Mensaje")]
    public GameObject messagePanel; // Arrastra el panel aquí desde el inspector
    public TextMeshProUGUI messageText; // Arrastra el TextMeshPro UGUI aquí
    public string message = "Debes recoger la manzana que hay en el mapa y no olvides usar E para recoger la espada, lo necesitarás";
    public float displayTime = 5f; // Tiempo en segundos que se muestra el mensaje

    void Start()
    {
        // Asegurarse de que el panel esté desactivado al inicio
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        // Iniciar la corutina para mostrar el mensaje
        StartCoroutine(ShowTutorialMessage());
    }

    IEnumerator ShowTutorialMessage()
    {
        // Esperar un frame para asegurarse de que todo está inicializado
        yield return null;

        // Activar el panel y asignar el texto
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = message;
        }

        // Esperar el tiempo designado
        yield return new WaitForSeconds(displayTime);

        // Desactivar el panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    // Opcional: Función para mostrar el mensaje manualmente
    public void ShowMessage(string customMessage = "", float customTime = 0f)
    {
        StartCoroutine(ShowCustomMessage(customMessage, customTime));
    }

    IEnumerator ShowCustomMessage(string customMessage, float customTime)
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }

        if (messageText != null)
        {
            messageText.text = string.IsNullOrEmpty(customMessage) ? message : customMessage;
        }

        yield return new WaitForSeconds(customTime > 0 ? customTime : displayTime);

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }
}