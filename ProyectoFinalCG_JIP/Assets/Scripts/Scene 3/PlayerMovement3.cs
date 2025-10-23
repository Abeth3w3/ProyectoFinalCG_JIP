using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Rotación Suave")]
    public float turnSmoothTime = 0.1f;
    private float turnSmoothVelocity;

    [Header("Referencias")]
    public Transform cameraTransform;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Animator animator;

    [Header("Detección de suelo")]
    public float groundDistance = 0.4f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 🔹 Verificar si está en el suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // 🔹 Movimiento con cámara
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(x, 0f, z).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

            // Rota solo el jugador (padre)
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Mueve al jugador hacia la dirección de la cámara
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Actualiza el parámetro Speed en el Animator
            animator.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        }


        // 🔹 Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // 🔹 Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Evita que el modelo se desplace solo
        if (animator != null)
        {
            animator.transform.localPosition = Vector3.zero;
        }

    }

}
