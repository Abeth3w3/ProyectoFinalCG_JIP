using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movimiento del jugador")]
    public float speed = 5f;
    public float rotationSmoothTime = 0.1f;

    [Header("Referencias")]
    public Transform cameraTransform;  // Asigna la cámara principal (Main Camera o Cinemachine)
    public Animator animator;          // Asigna el Animator del modelo (ej. JoeModel)

    private CharacterController controller;
    private float rotationVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();

        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Ejes de movimiento
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Si hay movimiento
        if (direction.magnitude >= 0.1f)
        {
            // Ángulo hacia el que el jugador debe rotar según la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Suavizar la rotación
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Dirección final del movimiento
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

            // Actualizar parámetro del Animator
            animator.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
        }
        else
        {
            animator.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
        }
    }
}
