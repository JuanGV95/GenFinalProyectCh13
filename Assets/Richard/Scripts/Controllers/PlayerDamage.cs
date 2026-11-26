using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    private int playerLife = 500;
    public static PlayerDamage instance;
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(playerLife);
    }
    public void setDamage(int damage)
    {
        playerLife = playerLife - damage;
        if(playerLife < 1)
        {
            gameObject.SetActive(false);
        }
    }
    
    public int GetLife()
    {
        return playerLife;
    }
}
