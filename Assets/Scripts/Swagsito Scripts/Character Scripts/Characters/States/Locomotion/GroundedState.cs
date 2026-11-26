public abstract class GroundedState : LocomotionState
{
    protected abstract float Speed { get; }

    protected GroundedState(HeroContext context)
        : base(context)
    {
    }

    public override void EnterState()
    {
        Move(Speed);
    }

    public override void Tick(float deltaTime)
    {
        if (TryAirborneTransition())
            return;

        IState nextState = States.GetGroundedState(Input);

        if (nextState != this)
        {
            Machine.ChangeState(nextState);
            return;
        }

        Move(Speed);
    }

    protected bool TryAirborneTransition()
    {
        if (!Motor.IsGrounded)
        {
            Machine.ChangeState(States.Fall);
            return true;
        }

        if (Input.JumpPressed && Motor.TryJump())
        {
            Machine.ChangeState(States.Jump);
            return true;
        }

        return false;
    }
}