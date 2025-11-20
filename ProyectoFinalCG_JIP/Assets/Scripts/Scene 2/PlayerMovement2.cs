using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
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

        // --- Entrada ---
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(horizontal, 0f, vertical).normalized;

        bool wantsToRun = Input.GetKey(KeyCode.LeftShift);
        bool wantsToCrouch = Input.GetKey(KeyCode.LeftControl);

        if (isCrouching && wantsToRun)
            ToggleCrouch(false);

        if (wantsToCrouch && isGrounded && !isCrouching)
            ToggleCrouch(true);
        else if (!wantsToCrouch && isCrouching)
            ToggleCrouch(false);

        float speed = isCrouching ? crouchSpeed : (wantsToRun ? runSpeed : walkSpeed);

        // --- Movimiento (NO rotación aquí) ---
        if (input.magnitude >= 0.1f)
        {
            // Usamos la dirección del jugador, no la rotación de la cámara
            Vector3 moveDir = transform.forward * vertical + transform.right * horizontal;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            animator.SetBool("isWalking", !wantsToRun && !isCrouching);
            animator.SetBool("isRunning", wantsToRun && !isCrouching);
            animator.SetBool("isCrouching", isCrouching);
        }
        else
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
         
        }

        // --- Salto ---
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouching)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetBool("isJumping", true);
        }

        // --- Gravedad ---
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void ToggleCrouch(bool crouch)
    {
        isCrouching = crouch;
        animator.SetBool("isCrouching", crouch);

        if (crouch)
        {
            float heightDiff = standingHeight - crouchingHeight;
            controller.height = crouchingHeight;
            controller.center = new Vector3(0, standingCenter.y - (heightDiff / 2f), 0);
        }
        else
        {
            controller.height = standingHeight;
            controller.center = standingCenter;
        }
    }
}
