using UnityEngine;

namespace TuNombre
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float speed = 6f;
        public float runSpeed = 10f;
        public float rotationSmoothness = 10f;

        [Header("Components")]
        public CharacterController controller;
        public Animator animator;

        void Update()
        {
            HandleMovement();
        }

        void HandleMovement()
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            bool isRunning = Input.GetKey(KeyCode.LeftShift);

            // Calcular dirección de movimiento
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            Vector3 moveDirection = (cameraForward * z) + (cameraRight * x);

            bool isMoving = moveDirection.magnitude > 0.1f;

            // ANIMACIONES SIMPLES - SOLO DETECTAR SI HAY MOVIMIENTO
            animator.SetBool("isWalking", isMoving && !isRunning);
            animator.SetBool("isRunning", isMoving && isRunning);
            animator.SetBool("isWalkingBack", false); // Por ahora no usar

            if (isMoving)
            {
                // Rotar hacia la dirección del movimiento
                if (moveDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.deltaTime);
                }

                // Mover
                float currentSpeed = isRunning ? runSpeed : speed;
                controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
        }
    }
}