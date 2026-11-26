using System.Collections;
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
    public int enemyLife { get ; set ; }

    public static Enemy1Manager instance; // Created and static instance reference for this class

    //private GameObject floor; // surface where the npc will move
    Collider floorCollider; //collider form the floor
    private NavMeshAgent agent;// to add navmesh by coding
    private const int divided = 2;// Const to divide bounds

    private Vector3 finalPosition ; // Final position to check

    private Coroutine coroutine; //test corrutine
    
    

    void Awake()
    {
        //It depends on floor tag
        floorCollider = GameObject.FindGameObjectWithTag("Piso").GetComponent<Collider>();
        
        instance = this; // proving features to make all methods 
       
        
    }

    void Start()
   {    //Created navMEshAgent by coding
        agent = gameObject.AddComponent<NavMeshAgent>();
        //test to check the npc movment   
        SetAlert(false);
        StartCoroutine(WaitingForSurface());
    }


    // Update is called once per frame
    void Update()
    {
       EnemyState();
    }



    public void SetAlert(bool enemyAlert)
    {
        this.enemyAlert = enemyAlert;// to set Enemy alert by AlertManager Class
    }

    public void SetAttack(int enemyAttack)
    {
       this.enemyAttack = enemyAttack;
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
    public void SetLife(int enemyLife)
    {
        this.enemyLife = enemyLife;
    }
    //Function to set a random range Between bounds
    private Vector3 setRandomMove()
    {
        //Initial variables to get surface limits
        float x = (floorCollider.bounds.size.x-1)/divided;
        float z = (floorCollider.bounds.size.z-1)/divided;
        //Random variables to  move agent over the surface
        float xPoint = Random.Range(-x,x);
        float zPoint = Random.Range(-z,z);
        return new Vector3(xPoint,0.58f,zPoint);
    }
    //Funcition to wait until agent is on navmesh
    private IEnumerator WaitingForSurface()
    {
         yield return new WaitUntil(() => agent.isOnNavMesh);
         finalPosition = setRandomMove();
        // to set first destination
        SetEnemyMove(finalPosition);
    }

    private IEnumerator WaitingEnemy()
    {
        yield return new WaitForSeconds(3f);
        //Created new random final point
        finalPosition = setRandomMove();
        // to set first destination
        SetEnemyMove(finalPosition);
    }
    

    //Function to determine if Enemy arrived to watch point
    public void PlayerArrived()
    {
        float distancia = Vector3.Distance(finalPosition,agent.transform.position);
        if (!agent.pathPending && distancia < 2f && agent.velocity.sqrMagnitude < 0.01f)
        {
            coroutine = StartCoroutine(WaitingEnemy());
        }
    }

     //Function to go to player
    public void StartAttack()
    {
        //to bring player position and replace finaalposition value
        finalPosition = AlertManager.instance.GetPlayerPosition();
        //to modify new position
        //float distancia = Vector3.Distance(finalPosition,agent.transform.position);
        SetEnemyMove(finalPosition);
    }

    private void EnemyState()
    {   //to detect when to change
        if (enemyAlert)
        {
 
            StartAttack();
          
        }
        else
        {
            PlayerArrived(); // To know when the Enemy arrived
        }
    }
  


   
}
