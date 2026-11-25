using UnityEngine;

public class HeroContext
{
    public HeroController Hero { get; }

    public HeroInputReader Input { get; }
    public HeroMotor Motor { get; }

    public Transform CameraTransform { get; }

    public StateMachine LocomotionMachine{ get; }
    public StateMachine ActionsMachine { get; }

    public HeroContext(HeroController hero,HeroInputReader input, HeroMotor motor, Transform cameraTransform, StateMachine locomotionMachine, StateMachine actionsMachine)
    {
        Hero = hero;
        Input = input;
        Motor = motor;
        CameraTransform = cameraTransform;
        LocomotionMachine = locomotionMachine;
        ActionsMachine = actionsMachine;
    }
}
