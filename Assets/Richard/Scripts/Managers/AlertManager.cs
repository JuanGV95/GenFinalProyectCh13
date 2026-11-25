using UnityEngine;

public class AlertManager : MonoBehaviour
{
    public GameObject player;// This is the current hero player 
    public GameObject watchingPoint; //this is the watchin point

    private Vector3 playerPositon;// To know where the player is
    private Vector3 watchingPointPosition; //to know where the whacher is
    public static AlertManager instance;
    private int radioDeAlerta = 2;
    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //get watchingPoint Position
        watchingPointPosition = watchingPoint.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Get player position
        playerPositon = player.transform.position;
        if (Vector3.Distance(playerPositon, watchingPointPosition) < radioDeAlerta)
        {
            Debug.Log("Enemy is comming");
            Enemy1Manager.instance.SetAlert(true);
        }
    }

    public Vector3 GetPlayerPosition()
    {
        return playerPositon;
    }
}
