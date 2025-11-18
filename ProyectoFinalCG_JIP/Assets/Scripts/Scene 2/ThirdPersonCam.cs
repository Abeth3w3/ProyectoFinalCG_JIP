using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smoothSpeed = 10f;

    // Variables para control de rotación
    private float currentXRotation = 0f;
    private float currentYRotation = 0f;
    public float mouseSensitivity = 100f;
    public float maxLookAngle = 80f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Inicializar rotaciones con la rotación actual de la cámara
        currentYRotation = transform.eulerAngles.y;
        currentXRotation = transform.eulerAngles.x;
    }

    void LateUpdate()
    {
        HandleCameraRotation();

        // Calcular posición deseada basada en la rotación actual
        Vector3 desiredOffset = Quaternion.Euler(currentXRotation, currentYRotation, 0) * offset;
        Vector3 desiredPosition = target.position + desiredOffset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 1.5f); // Mirar al centro del personaje
    }

    void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotación vertical
        currentXRotation -= mouseY;
        currentXRotation = Mathf.Clamp(currentXRotation, -maxLookAngle, maxLookAngle);

        // Rotación horizontal
        currentYRotation += mouseX;
    }
}