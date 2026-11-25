using UnityEngine;

[RequireComponent(typeof(HeroInputReader))]
[RequireComponent(typeof(HeroMotor))]
public class HeroController : MonoBehaviour
{

    [SerializeField] private Transform cameraTransform;

    private HeroInputReader input;
    private HeroMotor motor;
    public StateMachine LocomotionMachine { get; private set; }
    public StateMachine ActionsMachine { get; private set; }

    public HeroContext Context { get; private set; }

    private void Awake()
    {
        input = GetComponent<HeroInputReader>();
        motor = GetComponent<HeroMotor>();

        LocomotionMachine = new StateMachine();
        ActionsMachine = new StateMachine();

        Context = new HeroContext(this, input, motor, cameraTransform, LocomotionMachine, ActionsMachine);
    }

    private void Start()
    {
        LocomotionMachine.ChangeState(new IdleState(Context));
        ActionsMachine.ChangeState(new NeutralActionState(Context));
    }

    private void Update()
    {
        LocomotionMachine.Tick(Time.deltaTime);
        ActionsMachine.Tick(Time.deltaTime);

        TestMovement();
    }

    private void FixedUpdate()
    {
        motor.RefreshGround();

        LocomotionMachine.FixedTick(Time.fixedDeltaTime);
        ActionsMachine.FixedTick(Time.fixedDeltaTime);

        motor.FixedTick(Time.fixedDeltaTime);
    }

    private void TestMovement()
    {
        Vector3 direction =
            MovementUtility.GetCameraRelativeDirection(
                input.MoveInput,
                cameraTransform);

        float speed = input.SprintHeld
            ? motor.Data.sprintSpeed
            : motor.Data.runSpeed;

        motor.SetMovement(direction, speed);

        motor.SetFacingDirection(cameraTransform.forward);

        if (input.JumpPressed)
            motor.TryJump();
    }
}
