using UnityEngine;

public class JoeMovement : MonoBehaviour
{
    public Animator anim;
    public float speed = 5f;

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);

        // Actualizar parámetro Speed en el Animator
        anim.SetFloat("Speed", direction.magnitude);

        // Mover el personaje
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }
}
