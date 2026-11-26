public class FallState : AirborneState
{
    public FallState(HeroContext context)
        : base(context)
    {
    }

    public override void Tick(float deltaTime)
    {
        if (Motor.IsGrounded)
        {
            Machine.ChangeState(States.Land);
            return;
        }

        UpdateAirMovement();
    }
}