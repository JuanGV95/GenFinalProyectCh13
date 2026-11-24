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

    private int divided = 2;

    void Awake()
    {
        floorCollider = floor.GetComponent<Collider>();
    }
     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //test to check the npc movment
        Vector3 myDestination = new Vector3(-floorCollider.bounds.size.x/divided,1,-floorCollider.bounds.size.z/divided);
        agent.SetDestination(myDestination);
        
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
       
    }

    public void SetSpeed(float enemySpeed)
    {
        
    }


   
}
