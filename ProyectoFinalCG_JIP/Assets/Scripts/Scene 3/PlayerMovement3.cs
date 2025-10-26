using UnityEngine;

public class PlayerMovement3 : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;
    public Animator animator;

    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 10f;
    public float crouchSpeed = 3f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.5f;

    [Header("Crouch Settings")]
    public float standingHeight = 2f;
    public float crouchingHeight = 1.2f;
    public Vector3 standingCenter = new Vector3(0, 1f, 0);
    public Vector3 crouchingCenter = new Vector3(0, 0.6f, 0);

    Vector3 velocity;
    bool isGrounded;
    bool isCrouching;

    void Update()
    {
        // --- Detección del suelo ---
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("isJumping", false);
        }

        // --- Entrada de movimiento ---
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // --- Correr / Caminar / Agacharse ---
        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
        bool wantsToCrouch = Input.GetKey(KeyCode.LeftControl);

        // Si intenta correr mientras está agachado → se levanta
        if (isCrouching && wantsToRun)
        {
            ToggleCrouch(false);
        }

        // Control manual del agachado
        if (wantsToCrouch && isGrounded && !isCrouching)
        {
            ToggleCrouch(true);
        }
        else if (!wantsToCrouch && isCrouching)
        {
            ToggleCrouch(false);
        }

        // --- Determinar velocidad actual ---
        float currentSpeed = isCrouching ? crouchSpeed : (wantsToRun ? runSpeed : walkSpeed);

        // --- Movimiento y rotación ---
        if (direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = rotation;

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            animator.SetBool("isWalking", !wantsToRun && !isCrouching && isGrounded);
            animator.SetBool("isRunning", wantsToRun && !isCrouching && isGrounded);
            animator.SetBool("isCrouching", isCrouching && isGrounded);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            animator.SetBool("isCrouching", isCrouching && isGrounded);
        }

        // --- Salto ---
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }

        // --- Aplicar gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ToggleCrouch(bool crouch)
    {
        isCrouching = crouch;
        animator.SetBool("isCrouching", crouch);

        if (crouch)
        {
            // Reducir altura a la mitad (o al valor deseado)
            float heightDiff = standingHeight - crouchingHeight;
            controller.height = crouchingHeight;
            controller.center = new Vector3(0, standingCenter.y - (heightDiff / 2f), 0);
        }
        else
        {
            // Volver a la altura original
            float heightDiff = standingHeight - crouchingHeight;
            controller.height = standingHeight;
            controller.center = new Vector3(0, standingCenter.y, 0);
        }
    }
}



