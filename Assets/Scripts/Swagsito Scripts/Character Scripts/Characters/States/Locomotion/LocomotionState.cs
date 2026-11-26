using Unity.VisualScripting;
using UnityEngine;

public abstract class LocomotionState : HeroState
{
    protected HeroMotor Motor => Context.Motor;
    protected HeroInputReader Input => Context.Input;
    protected MovementData Data => Motor.Data;

    protected LocomotionStates States => Context.LocomotionStates;

    protected LocomotionState(HeroContext context) : base(context, context.LocomotionMachine)
    {
    }

    protected void Move(float speed)
    {
        Vector3 direction = MovementUtility.GetCameraRelativeDirection(Input.MoveInput, Context.CameraTransform);

        Motor.SetMovement(direction, speed);

        UpdateFacing();
    }

    protected void UpdateFacing()
    {
        Motor.SetFacingDirection(Context.CameraTransform.forward);
    }
}
