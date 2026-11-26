using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMananager : MonoBehaviour
{
    public GameObject enemies;
    private bool control = true;

    // Update is called once per frame
    void Update()
    {
        StartMyCourrutine();
    }

    public void StartMyCourrutine()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        if (Enemy1Manager.instance.enemyAlert && control)
        {
            control = false;
            for (int i = 0; i<5; i++)
            {
                //Instantiate(enemies,enemies.transform.position,enemies.transform.rotation); 
                Instantiate(enemies,new Vector3(6,0.5f,6),enemies.transform.rotation);   
                yield return new WaitForSeconds(1); 
            }
            
        }
    }
}
