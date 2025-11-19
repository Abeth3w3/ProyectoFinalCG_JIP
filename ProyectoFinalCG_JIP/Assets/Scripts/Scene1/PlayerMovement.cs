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
    public bool isInvincible = false; // ← NUEVO: Para evitar daño durante roll

    [Header("Rotación")]
    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    [Header("Mouse Look")] // ← NUEVA SECCIÓN
    public float mouseSensitivity = 2f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;
    private float verticalRotation = 0f;

    [Header("Ground Check")]
    public Transform GroundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();

        // ← NUEVO: Bloquear cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ← NUEVO: Rotación con mouse
        HandleMouseLook();

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

    // ← NUEVO MÉTODO: Rotación con mouse
    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotación horizontal (jugador)
        transform.Rotate(0, mouseX, 0);

        // Rotación vertical (cámara)
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, minVerticalAngle, maxVerticalAngle);

        cam.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
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
            // ← MODIFICADO: Ya no usa rotación suave porque ahora usamos mouse
            Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            moveAmount = isRunning ? 1f : 0.5f;
        }

        // --- Actualizar animaciones ---
        animator.SetFloat("Blend", moveAmount);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsRolling", isRolling);
    }

    void StartRoll()
    {
        isRolling = true;
        isInvincible = true; // ← NUEVO: Invencibilidad durante roll
        rollTimer = rollDuration;

        // Determinar dirección del roll
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            // Roll en dirección del movimiento
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            rollDirection = transform.forward * vertical + transform.right * horizontal;
        }
        else
        {
            // Roll hacia adelante si no hay input
            rollDirection = transform.forward;
        }

        rollDirection = rollDirection.normalized;
        animator.SetTrigger("Roll");

        // ← NUEVO: Programar fin del roll
        Invoke("EndRoll", rollDuration);
    }

    void HandleRoll()
    {
        rollTimer -= Time.deltaTime;
        controller.Move(rollDirection * rollSpeed * Time.deltaTime);
    }

    // ← NUEVO MÉTODO: Terminar roll
    void EndRoll()
    {
        isRolling = false;
        isInvincible = false;
    }

    // ← NUEVO MÉTODO: Para que otros scripts verifiquen invencibilidad
    public bool IsInvincible()
    {
        return isInvincible;
    }

    private void OnDrawGizmosSelected()
    {
        if (GroundCheck == null) return;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(GroundCheck.position, groundDistance);
    }
}