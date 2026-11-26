public class WalkState : GroundedState
{
    protected override float Speed => Data.walkSpeed;

    public WalkState(HeroContext context)
        : base(context)
    {
    }
}