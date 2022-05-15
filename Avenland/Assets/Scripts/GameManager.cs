using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameSettings settings;
    private RandomGeneration levelGeneration;

    private int dungeonSizeX;
    private int dungeonSizeY;

    private int minSmallDungeonSize = 25;
    private int maxSmallDungeonSize = 51;

    private int minMediumDungeonSize = 50;
    private int maxMediumDungeonSize = 101;

    private int minLargeDungeonSize = 100;
    private int maxLargeDungeonSize = 251;

    private GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        //If changing to scene switching, remove
        try
        {
            settings = GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameSettings>();
        }
        catch
        {
            settings = GameObject.FindGameObjectWithTag("GameOptionsTest").GetComponent<GameSettings>(); ;
        }

        levelGeneration = GameObject.FindGameObjectWithTag("LevelGenerator").GetComponent<RandomGeneration>();

        playerObject = GameObject.FindGameObjectWithTag("Player");

        int randX = 0;
        int randY = 0;

        switch (settings.dungeonSize)
        {
            case DungeonSize.SMALL:
                    randX = Random.Range(minSmallDungeonSize, maxSmallDungeonSize);
                    dungeonSizeX = randX;

                    randY = Random.Range(minSmallDungeonSize, maxSmallDungeonSize);
                    dungeonSizeY = randY;
                break;
            case DungeonSize.MEDIUM:
                    randX = Random.Range(minMediumDungeonSize, maxMediumDungeonSize);
                    dungeonSizeX = randX;

                    randY = Random.Range(minMediumDungeonSize, maxMediumDungeonSize);
                    dungeonSizeY = randY;
                break;
            case DungeonSize.LARGE:
                    randX = Random.Range(minLargeDungeonSize, maxLargeDungeonSize);
                    dungeonSizeX = randX;

                    randY = Random.Range(minLargeDungeonSize, maxLargeDungeonSize);
                    dungeonSizeY = randY;
                break;
        }

        int offsetX = (int)Random.Range(((int)settings.dungeonSize + 1f) * 5f, ((int)settings.dungeonSize + 1f) *5f);
        int offsetY = (int)Random.Range(((int)settings.dungeonSize + 1f) * 5f, ((int)settings.dungeonSize + 1f) *5f);

        Vector3 playerSpawnLocation = new Vector3(((dungeonSizeX / 2) + offsetX) * 1.28f, -(((dungeonSizeY / 2) + offsetY)* 1.28f), 0);

        playerObject.transform.position = playerSpawnLocation;

        levelGeneration.amountOfDoors = ((int)settings.dungeonSize + 1) * 2;

        levelGeneration.PickDoorwayLocations(dungeonSizeX, dungeonSizeY);

        levelGeneration.GenerateFloor(new Vector3(0, 0, 0), GenerationDirection.BOTTOM_RIGHT, FloorType.FLOOR_BLANK_1, dungeonSizeX, dungeonSizeY);
        levelGeneration.GenerateWalls(new Vector3(0, 0, 0), dungeonSizeX, dungeonSizeY);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
