public abstract class AirborneState : LocomotionState
{
    protected AirborneState(HeroContext context)
        : base(context)
    {
    }

    public override void EnterState()
    {
        UpdateAirMovement();
    }

    protected void UpdateAirMovement()
    {
        float speed = Input.SprintHeld ? Data.sprintSpeed : Data.runSpeed;

        Move(speed);
    }
}