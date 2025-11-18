using UnityEngine;

public class RotatingSkybox : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1f;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
