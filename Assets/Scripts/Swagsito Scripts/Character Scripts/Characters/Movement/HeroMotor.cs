using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

[RequireComponent(typeof(CapsuleCollider))]

[RequireComponent(typeof(HeroGroundDetector))]
public class HeroMotor : MonoBehaviour
{
    [SerializeField] private MovementData movementData;

    private Rigidbody rb;
    private HeroGroundDetector groundDetector;

    private Vector3 desiredVelocity;
    private Vector3 desiredFacing;

    private bool jumpRequested;
    public bool IsJumpPending => jumpRequested;

    private bool motorEnabled = true;

    public MovementData Data => movementData;

    public Vector3 Velocity => rb.linearVelocity;

    public float VerticalVelocity => Velocity.y;

    public float HorizontalSpeed => Vector3.ProjectOnPlane(Velocity, Vector3.up).magnitude;

    public bool IsGrounded => groundDetector.IsGrounded && Vector3.Dot(Velocity.normalized, groundDetector.GroundNormal) < 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        groundDetector = GetComponent<HeroGroundDetector>();

        rb.useGravity = false;

        desiredFacing = transform.forward;

        groundDetector.Initialize(movementData);
    }

    public void SetMovement(Vector3 direction, float speed)
    {
        direction.y = 0f;

        direction = Vector3.ClampMagnitude(direction, 1f);

        desiredVelocity = direction * Mathf.Max(0f, speed);
    }

    public void SetFacingDirection(Vector3 direction)
    {
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
            return;

        desiredFacing = direction.normalized;
    }

    public bool TryJump()
    {
        if (!IsGrounded || jumpRequested)
            return false;

        jumpRequested = true;

        return true;
    }

    public void RefreshGround()
    {
        groundDetector.CheckGround();
    }

    public void FixedTick(float fixedDeltaTime)
    {
        if (motorEnabled)
        {
            UpdateMovement(fixedDeltaTime);
            UpdateRotation(fixedDeltaTime);
        }

        if (jumpRequested)
        {
            ExecuteJump();
            jumpRequested = false;
        }

        ApplyGravity(fixedDeltaTime);
    }

    private void UpdateMovement(float deltaTime)
    {
        Vector3 normal = IsGrounded ? groundDetector.GroundNormal : Vector3.up;

        Vector3 targetVelocity = Vector3.ProjectOnPlane(desiredVelocity, normal);

        if (targetVelocity.sqrMagnitude > 0.0001f)
        {
            targetVelocity = targetVelocity.normalized
                * desiredVelocity.magnitude;
        }

        Vector3 currentVelocity =
            Vector3.ProjectOnPlane(Velocity, normal);

        float changeRate = desiredVelocity.sqrMagnitude > 0.0001f ? movementData.acceleration : movementData.deceleration;

        if (!IsGrounded)
            changeRate *= movementData.airControl;

        Vector3 velocityChange = targetVelocity - currentVelocity;

        velocityChange = Vector3.ClampMagnitude(velocityChange, changeRate * deltaTime);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void UpdateRotation(float deltaTime)
    {
        Quaternion targetRotation = Quaternion.LookRotation(desiredFacing);

        Quaternion nextRotation = Quaternion.RotateTowards(rb.rotation, targetRotation, movementData.rotationSpeed * deltaTime);

        rb.MoveRotation(nextRotation);
    }

    private void ExecuteJump()
    {
        if (!IsGrounded)
            return;

        float jumpVelocity = Mathf.Sqrt( 2f * movementData.gravity * movementData.jumpHeight);

        float velocityChange = jumpVelocity - VerticalVelocity;

        rb.AddForce(Vector3.up * velocityChange, ForceMode.VelocityChange);
    }

    private void ApplyGravity(float deltaTime)
    {
        float targetY = Mathf.Max( VerticalVelocity - movementData.gravity * deltaTime, - movementData.terminalVelocity);

        float velocityChange = targetY - VerticalVelocity;

        rb.AddForce(Vector3.up * velocityChange, ForceMode.VelocityChange);}

    public void StopMovement(bool immediate = false)
    {
        desiredVelocity = Vector3.zero;

        if (immediate)
        {
            rb.linearVelocity = Vector3.up * VerticalVelocity;
        }
    }

    public void AddImpulse(Vector3 impulse)
    {
        rb.AddForce(impulse, ForceMode.Impulse);
    }

    public void AddVelocityChange(Vector3 velocityChange)
    {
        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    public void SetMotorEnabled(bool value)
    {
        motorEnabled = value;

        if (!value)
        {
            desiredVelocity = Vector3.zero;
            jumpRequested = false;
        }
    }
}