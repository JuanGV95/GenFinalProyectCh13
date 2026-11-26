public class LandState : GroundedState
{
    private float elapsedTime;

    protected override float Speed => Input.SprintHeld ? Data.sprintSpeed : Input.WalkHeld ? Data.walkSpeed : Data.runSpeed;

    public LandState(HeroContext context)
        : base(context)
    {
    }

    public override void EnterState()
    {
        elapsedTime = 0f;

        base.EnterState();
    }

    public override void Tick(float deltaTime)
    {
        if (TryAirborneTransition())
            return;

        elapsedTime += deltaTime;

        Move(Speed);

        if (elapsedTime >= Data.landingDuration)
        {
            Machine.ChangeState(
                States.GetGroundedState(Input));
        }
    }
}