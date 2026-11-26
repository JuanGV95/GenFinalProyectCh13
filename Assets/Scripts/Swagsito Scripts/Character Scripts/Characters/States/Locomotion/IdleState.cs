using UnityEngine;

public class IdleState : GroundedState
{
    protected override float Speed => 0f;

    public IdleState(HeroContext context)
        : base(context)
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
