using UnityEngine;

public class HeroGroundDetector : MonoBehaviour
{
    [SerializeField] private Transform groundCheck;

    private MovementData data;

    public bool IsGrounded { get; private set; }

    public Vector3 GroundNormal { get; private set; } = Vector3.up;

    public void Initialize(MovementData movementData)
    {
        data = movementData;
    }

    public void CheckGround()
    {
        IsGrounded = false;
        GroundNormal = Vector3.up;

        if (!Physics.SphereCast(groundCheck.position, data.groundCheckRadius, Vector3.down, out RaycastHit hit, data.groundCheckDistance, data.groundLayer, QueryTriggerInteraction.Ignore))
        {
            return;
        }

        float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);

        IsGrounded = slopeAngle <= data.maxSlopeAngle;

        if (IsGrounded)

            GroundNormal = hit.normal;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        float radius = data != null ? data.groundCheckRadius : 0.25f;

        float distance = data != null ? data.groundCheckDistance : 0.15f;

        Gizmos.color = IsGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(groundCheck.position, radius);

        Gizmos.DrawWireSphere(groundCheck.position + Vector3.down * distance,radius);

        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * distance);
    }
}