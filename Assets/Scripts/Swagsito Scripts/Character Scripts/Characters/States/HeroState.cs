using UnityEngine;

public abstract class HeroState : IState
{
    protected readonly HeroContext Context;
    protected readonly StateMachine Machine;

    protected HeroState(HeroContext context, StateMachine machine)
    {
        Context = context;
        Machine = machine;
    }

    public virtual void EnterState() { }
    public virtual void ExitState() { }
    public virtual void Tick(float deltaTime) { }
    public virtual void FixedTick(float fixedDeltaTime) { }


}
