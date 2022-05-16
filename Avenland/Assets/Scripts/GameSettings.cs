using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public bool isHost;

    public int seed;
    public bool randomizeSeed;

    public DungeonSize dungeonSize;

    public int amountOfPlayers;

    public List<SpecializationType> chosenSpecializations;
    public List<string> playerNames;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool ValidSettings()
    {
        if (amountOfPlayers < 1)
        {
            return false;
        }
        else return true;
    }
}
