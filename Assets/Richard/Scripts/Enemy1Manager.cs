using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1Manager : MonoBehaviour, IEnemy
{
    public Vector3 enemyPosition { get ; set ; } 
    public int enemyDamage { get ; set; } 
    public float enemySpeed { get ; set ; }
    public float enemyAttack { get ; set; }
    public bool enemyAlert { get ; set ; }

    public GameObject floor; // surface where the npc will move
    Collider floorCollider; //collider form the floor

    public NavMeshAgent agent;
    private const int divided = 2;// Const to divide bounds
    

    void Awake()
    {
        floorCollider = floor.GetComponent<Collider>();
        
    }
     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //test to check the npc movment
        
        SetEnemyMove(setRandomMove());
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAlert(bool enemyAlert)
    {
        
    }

    public void SetAttack(int enemyAttack)
    {
       
    }

    public void SetDamage(int enemyDamage)
    {
        this.enemyDamage = enemyDamage;
    }

    public void SetEnemyMove(Vector3 enemyPosition)
    {
       agent.SetDestination(enemyPosition);
    }

    public void SetSpeed(float enemySpeed)
    {
        
    }
    //Function to set a random range Between bounds
    private Vector3 setRandomMove()
    {
        //Initial variables to get surface limits
        float x = floorCollider.bounds.size.x/divided;
        float z = floorCollider.bounds.size.z/divided;
        //Random variables to  move agent over the surface
        float xPoint = Random.Range(-x,x);
        float zPoint = Random.Range(-z,z);
        return new Vector3(xPoint,1,zPoint);
    }


   
}
