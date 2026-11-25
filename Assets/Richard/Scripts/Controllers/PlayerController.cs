using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }
    void OnEnable()
    {
        inputActions.Player.Move.Enable();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 playerDirections = inputActions.Player.Move.ReadValue<Vector2>();
        transform.Translate(Vector3.forward*5*playerDirections.y*Time.deltaTime);
        transform.Translate(Vector3.right*5*playerDirections.x*Time.deltaTime);
    }
}
