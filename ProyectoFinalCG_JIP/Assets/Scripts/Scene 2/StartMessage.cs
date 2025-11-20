using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartupMessage : MonoBehaviour
{
    [Header("UI References")]
    public GameObject messagePanel; // Panel que contiene el texto
    public TextMeshProUGUI messageText;
    public float displayTime = 5f; // Tiempo en segundos que se muestra el mensaje

    [Header("Message Content")]
    [TextArea(3, 5)]
    public string message = "Presiona E para recoger la espada, la necesitarás";

    void Start()
    {
        // Asegurarse de que el panel esté activo al inicio
        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }

        // Establecer el texto
        if (messageText != null)
        {
            messageText.text = message;
        }

        // Ocultar el mensaje después de displayTime segundos
        StartCoroutine(HideMessageAfterTime());
    }

    IEnumerator HideMessageAfterTime()
    {
        yield return new WaitForSeconds(displayTime);

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    // Opcional: Ocultar el mensaje si el jugador presiona una tecla
    void Update()
    {
        // Si el jugador presiona E, ocultar el mensaje antes de tiempo
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (messagePanel != null && messagePanel.activeSelf)
            {
                messagePanel.SetActive(false);
            }
        }
    }
}