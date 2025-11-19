using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Tu personaje
    public Vector3 targetOffset = new Vector3(0, 1.5f, 0);

    [Header("Camera Settings")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 100f;
    public float rotationSmoothTime = 0.1f;

    [Header("Limits")]
    public float minVerticalAngle = -30f;
    public float maxVerticalAngle = 70f;

    [Header("Smoothing")]
    public float positionSmoothTime = 0.1f;

    private float currentX = 0f;
    private float currentY = 0f;
    private Vector3 positionVelocity;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Inicializar rotación mirando al target
        if (target != null)
        {
            // Calcular ángulos iniciales basados en la posición actual
            Vector3 direction = (transform.position - target.position).normalized;
            currentX = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            currentY = Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        }
        else
        {
            Debug.LogError("ThirdPersonCamera: No hay target asignado!");
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: Target es null!");
            return;
        }

        HandleMouseInput();
        UpdateCameraPosition();
    }

    void HandleMouseInput()
    {
        // Rotación con mouse (solo si está presionado el botón derecho o el cursor está bloqueado)
        if (Input.GetMouseButton(1) || Cursor.lockState == CursorLockMode.Locked)
        {
            currentX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }

        // Limitar ángulo vertical
        currentY = Mathf.Clamp(currentY, minVerticalAngle, maxVerticalAngle);

        // Zoom con rueda del mouse
        distance -= Input.GetAxis("Mouse ScrollWheel") * 5f;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
    }

    void UpdateCameraPosition()
    {
        // Calcular posición deseada del target (incluyendo offset)
        Vector3 desiredTargetPosition = target.position + targetOffset;

        // Calcular rotación de la cámara
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);

        // Calcular posición deseada de la cámara
        Vector3 desiredCameraPosition = desiredTargetPosition + rotation * new Vector3(0, 0, -distance);

        // Aplicar suavizado a la posición
        transform.position = Vector3.SmoothDamp(transform.position, desiredCameraPosition, ref positionVelocity, positionSmoothTime);

        // Hacer que la cámara mire al target
        transform.LookAt(desiredTargetPosition);
    }

    // Para alternar el cursor (útil para debugging)
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    // Debug visual en el editor
    void OnDrawGizmosSelected()
    {
        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(target.position + targetOffset, 0.5f);
            Gizmos.DrawLine(target.position + targetOffset, transform.position);
        }
    }
}