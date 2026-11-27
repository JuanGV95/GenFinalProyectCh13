using System.Collections;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Enemy1Manager : MonoBehaviour, IEnemy
{
    public Vector3 enemyPosition { get ; set ; } 
    public int enemyDamage { get ; set; } 
    public float enemySpeed { get ; set ; }
    public int enemyAttack { get ; set; }
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
        SetLife(50);
        SetAttack(1);
        
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
       DetectingZone();
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
       //test
       agent.SetDestination(enemyPosition);
    }

    public void SetSpeed(float enemySpeed)
    {
        agent.speed = enemySpeed;
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
        SetFinalPosition(setRandomMove());
    }

    private IEnumerator WaitingEnemy()
    {
        yield return new WaitForSeconds(3f);
        //Created new random final point
        SetFinalPosition(setRandomMove());
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
        //test
        Vector3 vector3 = new Vector3(1,0,Random.Range(-1f,1f));
        SetFinalPosition(GetPlayerPosition()-vector3);
    }

      public void DetectingZone()
    {
        //get postition
        float attackingDistance = GetDistance();
        //to modify new position
        if (attackingDistance < 5f )
        {
            StartAttack();    
            StartCoroutine(ReduceEnemyLife(1));
            //the damage will only occure if the distan is fewer than 2 units
            if (attackingDistance<2f && PlayerDamage.instance.GetLife()>=0)
            {
            
                PlayerDamage.instance.setDamage(enemyAttack);
                
            }
            
        }
        
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
   

    private IEnumerator ReduceEnemyLife(int mytime)//Fuctiont to wait for making damage to player
    {
        yield return new WaitForSeconds(mytime);
    }

    private Vector3 GetPlayerPosition()//Function to get the current player postion from AlertMAnager class
    {
        return AlertManager.instance.GetPlayerPosition(); 
           
    }

    private float GetDistance()//To get distance between player and enemies
    {
        return Vector3.Distance(GetPlayerPosition(),agent.transform.position);
    }

    private void  SetFinalPosition(Vector3 finalposition) //To set a Random final position
    {
        this.finalPosition = finalposition;
        // to set first destination
        SetEnemyMove(finalPosition);
    }
  


   
}
