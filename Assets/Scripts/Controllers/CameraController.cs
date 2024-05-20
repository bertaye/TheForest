using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    public float movementSpeed = 5.0f;
    public float lookSpeed = 2.0f;
    public float maxLookAngle = 85.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    //These are the Vectors cached from Update method to get a more sensable Input
    Vector3 moveUpdateVector;
    Quaternion rotationUpdateQuat = Quaternion.identity;
    Rigidbody cameraRB;
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraRB = GetComponent<Rigidbody>();
        ConfigureRigidbody();
    }

    void ConfigureRigidbody()
    {
        cameraRB.useGravity = false;
        cameraRB.isKinematic = false;
        cameraRB.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void FixedUpdate()
    {
        cameraRB.MovePosition(cameraRB.position + (moveUpdateVector * Time.fixedDeltaTime * movementSpeed));

        cameraRB.MoveRotation(rotationUpdateQuat);

        cameraRB.velocity = Vector3.zero;
        cameraRB.angularVelocity = Vector3.zero;

        moveUpdateVector = Vector3.zero;
    }

    void Update()
    {
        HandleMovement();
        HandleLook();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    void HandleMovement()
    {
        float moveForward = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;
        float moveRight = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;

        moveUpdateVector = (transform.right * moveRight + transform.forward * moveForward).normalized;
    }

    void HandleLook()
    {
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;

        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        rotationUpdateQuat =  Quaternion.Euler(pitch, yaw, 0.0f);
    }
}
