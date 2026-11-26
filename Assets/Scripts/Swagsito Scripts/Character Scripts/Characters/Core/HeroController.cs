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
        motor.RefreshGround();

        LocomotionMachine.ChangeState(motor.IsGrounded ? Context.LocomotionStates.Idle : Context.LocomotionStates.Fall);
        ActionsMachine.ChangeState(new NeutralActionState(Context));
    }

    private void Update()
    {
        LocomotionMachine.Tick(Time.deltaTime);
        ActionsMachine.Tick(Time.deltaTime);
       
    }

    private void FixedUpdate()
    {
        motor.RefreshGround();

        LocomotionMachine.FixedTick(Time.fixedDeltaTime);
        ActionsMachine.FixedTick(Time.fixedDeltaTime);

        motor.FixedTick(Time.fixedDeltaTime);
    }

}
