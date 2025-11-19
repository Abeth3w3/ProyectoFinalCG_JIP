using UnityEngine;
using TMPro;
using System.Collections;

public class StartMessage : MonoBehaviour
{
    [Header("Configuración del Mensaje")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    public string message = "Debes recoger la manzana que hay en el mapa y no olvides usar E para recoger la espada, lo necesitarás";
    public float displayTime = 5f;

    // Evento estático para cuando el mensaje termina
    public static System.Action OnMessageEnd;

    void Start()
    {
        // Asegurarse de que el panel esté desactivado al inicio
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        // Iniciar la corrutina para mostrar el mensaje
        StartCoroutine(ShowTutorialMessage());
    }

    IEnumerator ShowTutorialMessage()
    {
        // Esperar un frame
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

        // Notificar que el mensaje terminó
        OnMessageEnd?.Invoke();
    }
}