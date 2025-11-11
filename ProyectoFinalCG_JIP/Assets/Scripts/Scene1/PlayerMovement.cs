using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    private Animator animator;

    [Header("Movimiento")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float gravity = -9.81f;
    public float jumpHeight = 2f;

    [Header("Rodar")]
    public float rollSpeed = 8f;
    public float rollDuration = 0.5f;
    private float rollTimer = 0f;
    private bool isRolling = false;
    private Vector3 rollDirection;

    [Header("Rotación")]
    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- Detección de suelo ---
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // --- Rodar ---
        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isRolling)
        {
            StartRoll();
        }

        if (isRolling)
        {
            HandleRoll();
            return; // Salir temprano para evitar otros movimientos durante el roll
        }

        // --- Movimiento normal ---
        HandleMovement();

        // --- Salto ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // --- Aplicar gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        float moveAmount = 0f;

        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            moveAmount = isRunning ? 1f : 0.5f;
        }

        // --- Actualizar animaciones ---
        animator.SetFloat("Blend", moveAmount);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsRunning", isRolling);
    }

    void StartRoll()
    {
        isRolling = true;
        rollTimer = rollDuration;

        // Determinar dirección del roll
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            // Roll en dirección del movimiento
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            rollDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            // Roll hacia adelante si no hay input
            rollDirection = transform.forward;
        }

        animator.SetTrigger("Roll");
    }

    void HandleRoll()
    {
        rollTimer -= Time.deltaTime;

        // Mover durante el roll
        controller.Move(rollDirection * rollSpeed * Time.deltaTime);

        if (rollTimer <= 0f)
        {
            isRolling = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(GroundCheck.position, groundDistance);
    }
}