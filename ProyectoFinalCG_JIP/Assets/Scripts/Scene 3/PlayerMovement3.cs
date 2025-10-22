using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // 🔸 Nueva referencia: la cámara
    public Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Verifica si está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // Movimiento del jugador
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // 🔸 Dirección basada en la cámara
        Vector3 move = cameraTransform.forward * z + cameraTransform.right * x;
        move.y = 0f; // evita que el jugador se incline

        // Mover
        controller.Move(move.normalized * speed * Time.deltaTime);

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
