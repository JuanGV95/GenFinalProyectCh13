using UnityEngine;

public static class MovementUtility
{
    public static Vector3 GetCameraRelativeDirection(Vector2 input, Transform cameraTransform)
    {
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up);

        if (forward.sqrMagnitude < 0.0001f)
        {
            Vector3 cameraRight = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

            forward = Vector3.Cross(cameraRight, Vector3.up);
        }

        forward.Normalize();

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        Vector3 direction = forward * input.y + right * input.x;

        return Vector3.ClampMagnitude(direction, 1f);
    }
}