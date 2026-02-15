using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Vector3 velocity;
    private Vector3 movementInput;
    private Vector2 mouseInput;
    private float xRotation;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private CharacterController controller;
    [Space]
    [SerializeField] private float speed;
    [SerializeField] private float sensitivity;

    void Update()
    {
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer();

        if (Input.GetMouseButton(1))
            MoveCamera();
    }

    private void MovePlayer()
    {
        Vector3 MoveVector = transform.TransformDirection(movementInput);

        if (Input.GetKey(KeyCode.Space))
        {
            velocity.y = 1f;
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            velocity.y = -1f;
        }

        controller.Move(MoveVector * speed * Time.deltaTime);
        controller.Move(velocity * speed * Time.deltaTime);

        velocity.y = 0f;
    }

    private void MoveCamera()
    {
        xRotation -= mouseInput.y * sensitivity;
        transform.Rotate(0f, mouseInput.x * sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}