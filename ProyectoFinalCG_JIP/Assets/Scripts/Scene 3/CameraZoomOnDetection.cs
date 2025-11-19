
using Unity.Cinemachine;
using UnityEngine;

public class CameraZoomOnDetection : MonoBehaviour
{
    [Header("Cinemachine Camera (3.x)")]
    public CinemachineCamera virtualCamera;

    [Header("Zoom Settings")]
    public float zoomedFOV = 30f;
    public float normalFOV = 40f;
    public float zoomSpeed = 5f;

    float currentFOV;
    float targetFOV;

    void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineCamera>();

        currentFOV = virtualCamera.Lens.FieldOfView;
        targetFOV = normalFOV;
    }

    void Update()
    {
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomSpeed);

        // Aplicar FOV (Cinemachine 3.x)
        var lens = virtualCamera.Lens;
        lens.FieldOfView = currentFOV;
        virtualCamera.Lens = lens;
    }

    public void ZoomIn()
    {
        targetFOV = zoomedFOV;
    }

    public void ZoomOut()
    {
        targetFOV = normalFOV;
    }
}
