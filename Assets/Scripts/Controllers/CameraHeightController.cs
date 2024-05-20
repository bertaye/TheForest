using UnityEngine;

public class CameraHeightController : MonoBehaviour
{
    [SerializeField][Tooltip("Below this height script start dispatching compute shader to prevent collision with terrain")] float HeightValueForCheck = 2.0f;
    [SerializeField] ComputeShader SimplexNoiseHeightShader;
    [SerializeField] ComputeShader PerlinNoiseHeightShader;
    [SerializeField][Tooltip("Must be same with Noise Shader's Smoothness value")] float NoiseSmoothness = 50.0f;
    [SerializeField] float HeightLimitOffset = 0.2f;

    private ComputeBuffer resultBuffer;
    private float[] resultData;


    [SerializeField] float movementSpeed = 5.0f;
    [SerializeField] float lookSpeed = 2.0f;
    [SerializeField] float maxLookAngle = 85.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    //These are the Vectors cached from Update method to get a more sensable Input
    Vector3 moveUpdateVector;
    Quaternion rotationUpdateQuat = Quaternion.identity;
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        resultData = new float[1];
        resultBuffer = new ComputeBuffer(1, sizeof(float));
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
        gameObject.transform.position += moveUpdateVector;
        float heightLimit = GetHeightAtPosition(gameObject.transform.position, NoiseSmoothness);
        if(gameObject.transform.position.y <= 0.5f && (gameObject.transform.position.y - HeightLimitOffset < heightLimit))
        {
            gameObject.transform.position = new Vector3(gameObject.transform.position.x, heightLimit + HeightLimitOffset, gameObject.transform.position.z);
        }
    }

    void HandleLook()
    {
        yaw += Input.GetAxis("Mouse X") * lookSpeed;
        pitch -= Input.GetAxis("Mouse Y") * lookSpeed;

        pitch = Mathf.Clamp(pitch, -maxLookAngle, maxLookAngle);

        rotationUpdateQuat = Quaternion.Euler(pitch, yaw, 0.0f);
        gameObject.transform.rotation = rotationUpdateQuat;
    }

    public float GetHeightAtPosition(Vector3 worldPosition, float smoothness)
    {
        // Set the position and smoothness buffer
        int kernelHandle = SimplexNoiseHeightShader.FindKernel("CSMain");
        SimplexNoiseHeightShader.SetVector("worldPosition", worldPosition);
        SimplexNoiseHeightShader.SetFloat("smoothness", smoothness);

        // Set the result buffer
        SimplexNoiseHeightShader.SetBuffer(kernelHandle, "resultBuffer", resultBuffer);

        // Dispatch the compute shader
        SimplexNoiseHeightShader.Dispatch(kernelHandle, 1, 1, 1);

        // Read back the result
        resultBuffer.GetData(resultData);

        return resultData[0];
    }
}
