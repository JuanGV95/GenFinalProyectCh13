using UnityEngine;

public interface IEnemy
{
    //Variables in common to enemies
    Vector3 enemyPosition {get; set;}
    int enemyDamage {get; set;}
    float enemySpeed {get; set;}
    float enemyAttack {get; set;}
    bool enemyAlert   {get; set;}

    //methods

    // to set enemy movements
    void SetEnemyMove(Vector3 enemyPosition);
    // to set how many damage enemy has recived
    void SetDamage(int enemyDamage);
    //to set enemy speed
    void SetSpeed(float enemySpeed);
    //to set the force for enemy attack
    void SetAttack(int enemyAttack);
    //to set when the enemy needs to change its state
    void SetAlert(bool enemyAlert);
}
