using System.Collections.Generic;
using UnityEngine;

public class SpawnMananager : MonoBehaviour
{
    public GameObject enemies;
    private bool control = true;
    //private List<GameObject> enemiesPool;
    void Awake()
    {
       
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        if (Enemy1Manager.instance.enemyAlert && control)
        {
            for (int i = 0; i<5; i++)
            {
                Instantiate(enemies,new Vector3(enemies.transform.position.x+i+1,1,enemies.transform.position.z+i+1),enemies.transform.rotation);    
            }
            control = false;
        }
    }
}
