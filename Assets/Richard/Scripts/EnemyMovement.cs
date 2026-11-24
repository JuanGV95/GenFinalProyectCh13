using UnityEngine;

public class EnemyMovement : MonoBehaviour, IEnemy
{
    public Vector3 enemyPosition { get ; set ; } 
    public int enemyDamage { get ; set; } 
    public float enemySpeed { get ; set ; }
    public float enemyAttack { get ; set; }
    public bool enemyAlert { get ; set ; }


     // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetDamage(4500);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(enemyDamage);
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
