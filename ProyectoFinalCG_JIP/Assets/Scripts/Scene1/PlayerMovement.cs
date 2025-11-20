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
    public bool isInvincible = false;

    [Header("Rotación")]
    public float turnSmoothTime = 0.2f;
    private float turnSmoothVelocity;

    [Header("Mouse Look")]
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

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMouseLook();

        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        if (Input.GetKeyDown(KeyCode.C) && isGrounded && !isRolling)
        {
            StartRoll();
        }

        if (isRolling)
        {
            HandleRoll();
            return;
        }

        HandleMovement();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(0, mouseX, 0);

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
            Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            moveAmount = isRunning ? 1f : 0.5f;
        }

        animator.SetFloat("Blend", moveAmount);
        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsJumping", !isGrounded);
        animator.SetBool("IsRolling", isRolling);
    }

    void StartRoll()
    {
        isRolling = true;
        isInvincible = true;
        rollTimer = rollDuration;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;
            rollDirection = transform.forward * vertical + transform.right * horizontal;
        }
        else
        {
            rollDirection = transform.forward;
        }

        rollDirection = rollDirection.normalized;

        
        animator.SetBool("IsRolling", true);

        Invoke("EndRoll", rollDuration);
    }

    void HandleRoll()
    {
        rollTimer -= Time.deltaTime;
        controller.Move(rollDirection * rollSpeed * Time.deltaTime);
    }

    void EndRoll()
    {
        isRolling = false;
        isInvincible = false;

        
        animator.SetBool("IsRolling", false);
    }

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
