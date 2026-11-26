using UnityEngine;

public class LocomotionStates
{
    public IdleState Idle { get; private set; }
    public WalkState Walk { get; private set; }
    public RunState Run { get; private set; }
    public SprintState Sprint { get; private set; }
    public JumpState Jump { get; private set; }
    public FallState Fall { get; private set; }
    public LandState Land { get; private set; }

    public LocomotionStates(HeroContext context)
    {
        Idle = new IdleState(context);
        Walk = new WalkState(context);
        Run = new RunState(context);
        Sprint = new SprintState(context);
        Jump = new JumpState(context);
        Fall = new FallState(context);
        Land = new LandState(context);
    }

    public IState GetGroundedState(HeroInputReader input)
    {
        if (input.MoveInput.sqrMagnitude < 0.0015f) return Idle;

        if(input.SprintHeld) return Sprint;

        if(input.WalkHeld) return Walk;

        return Run;
    }
}