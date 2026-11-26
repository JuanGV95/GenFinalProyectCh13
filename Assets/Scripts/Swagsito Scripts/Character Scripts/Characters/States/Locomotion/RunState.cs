public class RunState : GroundedState
{
    protected override float Speed => Data.runSpeed;

    public RunState(HeroContext context)
        : base(context)
    {
    }
}