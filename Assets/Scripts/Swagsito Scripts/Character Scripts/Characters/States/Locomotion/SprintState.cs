public class SprintState : GroundedState
{
    protected override float Speed => Data.sprintSpeed;

    public SprintState(HeroContext context)
        : base(context)
    {
    }
}