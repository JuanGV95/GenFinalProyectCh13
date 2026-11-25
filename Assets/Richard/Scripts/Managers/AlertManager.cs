using UnityEngine;

public class AlertManager : MonoBehaviour
{
    public GameObject player;// This is the current hero player 
    public GameObject watchingPoint; //this is the watchin point

    private Vector3 playerPositon;// To know where the player is
    private Vector3 watchingPointPosition; //to know where the whacher is
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        if (Vector3.Distance(playerPositon, watchingPointPosition) < 1f)
        {
            Debug.Log("Enemy is comming");
        }
    }
}
