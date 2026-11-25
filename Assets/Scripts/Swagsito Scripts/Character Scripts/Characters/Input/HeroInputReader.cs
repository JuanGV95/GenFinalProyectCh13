using UnityEngine;

public class HeroInputReader : MonoBehaviour
{
    private HeroControls controls;

    public Vector2 MoveInput => controls.Gameplay.Move.ReadValue<Vector2>();

    public Vector2 LookInput => controls.Gameplay.Look.ReadValue<Vector2>();

    public bool SprintHeld => controls.Gameplay.Sprint.IsPressed();

    public bool DefenseHeld => controls.Gameplay.Defense.IsPressed();

    public bool AttackHeld => controls.Gameplay.Attack.IsPressed();

    public bool JumpPressed => controls.Gameplay.Jump.WasPressedThisFrame();

    public bool AttackPressed => controls.Gameplay.Attack.WasPressedThisFrame();

    public bool DodgePressed => controls.Gameplay.Dodge.WasPressedThisFrame();

    public bool Ability1Pressed => controls.Gameplay.Ability1.WasPressedThisFrame();

    public bool Ability2Pressed => controls.Gameplay.Ability2.WasPressedThisFrame();

    public bool Ability3Pressed => controls.Gameplay.Ability3.WasPressedThisFrame();

    private void Awake()
    {
        controls = new HeroControls();
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void OnDestroy()
    {
        controls?.Dispose();
    }
}