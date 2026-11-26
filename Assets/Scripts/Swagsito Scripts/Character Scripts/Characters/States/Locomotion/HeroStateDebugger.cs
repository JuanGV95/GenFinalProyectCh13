using UnityEngine;

[RequireComponent(typeof(HeroController))]
public class HeroStateDebugger : MonoBehaviour
{
    [SerializeField] private bool logTransitions;

    [Header("Current States")]
    [SerializeField] private string locomotionState;
    [SerializeField] private string actionState;

    private HeroController hero;

    private void Awake()
    {
        hero = GetComponent<HeroController>();
    }

    private void LateUpdate()
    {
        string nextLocomotion = hero.LocomotionMachine
            .CurrentState?.GetType().Name ?? "None";

        string nextAction = hero.ActionsMachine
            .CurrentState?.GetType().Name ?? "None";

        if (logTransitions)
        {
            if (locomotionState != nextLocomotion)
            {
                Debug.Log(
                    $"Locomotion: {locomotionState} -> {nextLocomotion}");
            }

            if (actionState != nextAction)
            {
                Debug.Log(
                    $"Action: {actionState} -> {nextAction}");
            }
        }

        locomotionState = nextLocomotion;
        actionState = nextAction;
    }
}