using System;
using UnityEngine;

public class StateMachine
{
    public IState CurrentState { get; private set; }

    public event Action<IState, IState> OnStateChanged;

    public void ChangeState(IState newState)
    {
        if(newState == null)
        {
            throw new ArgumentNullException(nameof(newState));           
        }

        if (CurrentState == newState) return;
        
        IState previousState = CurrentState;

        CurrentState?.ExitState();      
        CurrentState = newState;
        CurrentState.EnterState();
        OnStateChanged?.Invoke(previousState, CurrentState);
    }

    public void Tick(float deltaTime)
    {
        CurrentState?.Tick(deltaTime);
    }

    public void FixedTick(float fixedDeltaTime)
    {
        CurrentState?.FixedTick(fixedDeltaTime);
    }
}
