using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpHeight = 2f;
    public float gravity = -9.81f;

    [Header("References")]
    public Transform cam;
    public Transform joeModel; // Modelo de Joe
    public Animator animator;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private bool isRunning;
    
   
    

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // --- Comprobar si está en el suelo ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Movimiento horizontal ---
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // --- Detectar si está corriendo ---
        isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (direction.magnitude >= 0.1f)
        {
            // Dirección según cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Movimiento del CharacterController
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Rotar modelo Joe sin root motion
            joeModel.rotation = Quaternion.Slerp(joeModel.rotation, Quaternion.Euler(0f, targetAngle, 0f), 10f * Time.deltaTime);

            // Animaciones de caminar o correr
            animator.SetFloat("Speed", currentSpeed);
            animator.SetBool("isRunning", isRunning);
            animator.SetBool("isJumping", false);
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        // --- Saltar ---
        if (Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }

        // --- Aplicar gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // --- Control de aterrizaje ---
        if (isGrounded && animator.GetBool("isJumping"))
        {
            animator.SetBool("isJumping", false);
        }
    }
}
