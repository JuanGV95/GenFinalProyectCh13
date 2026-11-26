using UnityEngine;

public class NeutralActionState : HeroState
{
    public NeutralActionState(HeroContext context) : base(context, context.ActionsMachine)
    {
    }

    public override void EnterState()
    {
        Debug.Log("El personaje entra un estado neutral");
    }

    public override void ExitState()
    {
        Debug.Log("El personaje sale del estado neutral");
    }
}