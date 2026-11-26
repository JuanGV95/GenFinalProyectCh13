using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    private float currentDistance = -1f;

    public Vector3 ResolvePosition(Vector3 origin, Vector3 desiredPosition, CameraData data, float deltaTime)
    {
        Vector3 offset = desiredPosition - origin;

        float desiredDistance = offset.magnitude;

        if (desiredDistance < 0.001f)
            return origin;

        Vector3 direction = offset / desiredDistance;

        float allowedDistance = desiredDistance;

        if (Physics.CheckSphere(origin, data.collisionRadius, data.obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            allowedDistance = 0f;
        }
        else if (Physics.SphereCast(origin, data.collisionRadius, direction, out RaycastHit hit, desiredDistance, data.obstacleLayers, QueryTriggerInteraction.Ignore))
        {
            allowedDistance = Mathf.Max(0f, hit.distance - data.collisionPadding);
        }

        if (currentDistance < 0f ||
            allowedDistance < currentDistance)
        {
            currentDistance = allowedDistance;
        }
        else
        {
            currentDistance = Mathf.MoveTowards(currentDistance, allowedDistance, data.collisionRecoverySpeed * deltaTime);
        }

        return origin + direction * currentDistance;
    }

    public void ResetCollision()
    {
        currentDistance = -1f;
    }
}