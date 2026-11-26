using UnityEngine;

[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(CameraCollisionHandler))]
[DefaultExecutionOrder(-100)]
public class HeroCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private HeroInputReader input;
    [SerializeField] private CameraData cameraData;

    private Camera controlledCamera;
    private CameraCollisionHandler collisionHandler;

    private Vector3 followPosition;
    private Vector3 followVelocity;

    private float yaw;
    private float pitch;

    public float Yaw => yaw;
    public float Pitch => pitch;

    public Vector3 AimDirection => transform.forward;

    private void Awake()
    {
        controlledCamera = GetComponent<Camera>();

        collisionHandler = GetComponent<CameraCollisionHandler>();
    }

    private void Start()
    {
        yaw = target.eulerAngles.y;

        pitch = Mathf.Clamp(cameraData.initialPitch, cameraData.minPitch, cameraData.maxPitch);

        followPosition = target.position;

        controlledCamera.fieldOfView = cameraData.fieldOfView;

        SetCursorLocked(true);

        UpdateRotation();
        UpdatePosition(0f);
    }

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        ReadLookInput(Time.deltaTime);

        UpdateRotation();
    }

    private void LateUpdate()
    {
        float deltaTime = Time.deltaTime;

        UpdateFollowPosition(deltaTime);
        UpdatePosition(deltaTime);
    }

    private void ReadLookInput(float deltaTime)
    {
        Vector2 look = input.LookInput;

        float sensitivityX = input.IsMouseLook ? cameraData.mouseSensitivityX : cameraData.gamepadSensitivityX * deltaTime;

        float sensitivityY = input.IsMouseLook ? cameraData.mouseSensitivityY : cameraData.gamepadSensitivityY * deltaTime;

        yaw += look.x * sensitivityX;

        float verticalInput = look.y * sensitivityY;

        pitch += cameraData.invertY ? verticalInput : -verticalInput;

        yaw = Mathf.Repeat(yaw, 360f);

        pitch = Mathf.Clamp(pitch, cameraData.minPitch, cameraData.maxPitch);
    }

    private void UpdateRotation()
    {
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    private void UpdateFollowPosition(float deltaTime)
    {
        if (cameraData.followSmoothTime <= 0f)
        {
            followPosition = target.position;
            return;
        }

        followPosition = Vector3.SmoothDamp(followPosition, target.position, ref followVelocity, cameraData.followSmoothTime, Mathf.Infinity, deltaTime);
    }

    private void UpdatePosition(float deltaTime)
    {
        Vector3 offset = new Vector3(cameraData.shoulderOffset, cameraData.verticalOffset, - cameraData.distance);

        Vector3 desiredPosition = followPosition + transform.rotation * offset;

        transform.position = collisionHandler.ResolvePosition(target.position, desiredPosition, cameraData, deltaTime);
    }

    public Ray GetAimRay()
    {
        return controlledCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    public void SetCursorLocked(bool locked)
    {
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;    

        Cursor.visible = !locked;
    }

    private void OnDisable()
    {
        SetCursorLocked(false);
    }
}