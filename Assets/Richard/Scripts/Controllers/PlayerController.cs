using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    private Rigidbody rigidbody;
    private Vector2 playerDirections;
    
    void Awake()
    {
        inputActions = new InputSystem_Actions();
        rigidbody = gameObject.GetComponent<Rigidbody>();
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
        playerDirections = inputActions.Player.Move.ReadValue<Vector2>();
        
    }
    void FixedUpdate()
    {
        rigidbody.AddForce(Vector3.forward*playerDirections.y,ForceMode.Impulse);
        rigidbody.AddForce(Vector3.right*playerDirections.x,ForceMode.Impulse);
    }
}
