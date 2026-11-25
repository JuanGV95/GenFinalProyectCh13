using UnityEngine;

public class IdleState : HeroState
{
    public IdleState(HeroContext context) : base (context, context.LocomotionMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log("Entrando a Idle State");
    }

    public override void ExitState()
    {
        Debug.Log("Saliendo de Idle State");
    }
}
