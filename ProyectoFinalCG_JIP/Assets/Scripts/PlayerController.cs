using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public CharacterController controller;
    public Animator animator;

    void Start()
    {
        if (controller == null)
            controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        bool isMoving = move.magnitude > 0.1f;
        animator.SetBool("isWalking", isMoving);
    }
}
