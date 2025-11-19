using UnityEngine;

public class AppleTrigger : MonoBehaviour
{
    public GameObject succubus; // La referencia al NPC
    public GameObject player;   // Jugador o lo que tengas

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            succubus.SetActive(true);  // Activa la súcubo
            Destroy(gameObject);       // Destruye la manzana
        }
    }
}
