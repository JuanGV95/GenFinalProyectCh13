using UnityEngine;

public interface IState
{
    void EnterState();
    void ExitState();

    void Tick(float deltaTime);

    void FixedTick(float fixedDeltaTime);
}
