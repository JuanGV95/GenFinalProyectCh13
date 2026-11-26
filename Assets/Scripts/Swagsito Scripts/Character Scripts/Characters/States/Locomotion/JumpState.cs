public class JumpState : AirborneState
{
    public JumpState(HeroContext context)
        : base(context)
    {
    }

    public override void Tick(float deltaTime)
    {
        UpdateAirMovement();

        if (Motor.IsJumpPending)
            return;

        if (Motor.VerticalVelocity <= 0f)
        {
            Machine.ChangeState(Motor.IsGrounded ? States.Land : States.Fall);
        }
    }
}