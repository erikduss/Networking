using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonExit : MonoBehaviour
{
    private float checkRange = 1.5f;

    private GameManager gameManager;
    public Vector2 doorLocation;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        doorLocation = new Vector2(transform.position.x / 1.28f, transform.position.y / 1.28f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool LookForNearbyPlayer(Vector2 playerLocation)
    {
        if (Vector2.Distance(doorLocation, playerLocation) <= checkRange)
        {
            Debug.Log("Close enough to door!");
            return true;
        }
        else
        {
            return false;
        }
    }
}
