using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RandomGeneration : MonoBehaviour
{
    private float tileSize = 1.28f; //Every tile is 128x128px which translates to 1.28f in unity

    private int minDistBetweenDoors = 20;
    public int amountOfDoors = 0;

    private int minDistanceBetweenEnemies = 10;
    public int amountOfEnemies = 0;

    //Floor Tiles
    [SerializeField] private List<GameObject> floorTiles = new List<GameObject>();

    //Walls
    [SerializeField] private List<GameObject> doorways = new List<GameObject>();
    public List<Vector2> doorLocations = new List<Vector2>(); 

    [SerializeField] private GameObject wall_top_left_corner;
    [SerializeField] private GameObject wall_top_right_corner;
    [SerializeField] private GameObject wall_top_middle;

    [SerializeField] private GameObject wall_bottom_left_corner;
    [SerializeField] private GameObject wall_bottom_right_corner;
    [SerializeField] private GameObject wall_bottom_middle;

    [SerializeField] private GameObject wall_left_middle;
    [SerializeField] private GameObject wall_right_middle;

    [SerializeField] private List<GameObject> topWalls = new List<GameObject>();

    [SerializeField] private List<GameObject> leftWalls = new List<GameObject>();

    [SerializeField] private List<GameObject> rightWalls = new List<GameObject>();

    [SerializeField] private List<GameObject> bottomWalls = new List<GameObject>();

    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        minDistBetweenDoors = minDistBetweenDoors * amountOfDoors;
        minDistanceBetweenEnemies = minDistanceBetweenEnemies * amountOfEnemies;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PickDoorwayLocations(float lengthX, float lengthY)
    {
        for(int i = 0; i< amountOfDoors; i++)
        {
            int randX = (int)Random.Range(3, (lengthX-3));
            int randY = (int)Random.Range(3, (lengthY-3));

            bool canSpawnLeft = true;
            bool canSpawnRight = true;
            bool canSpawnTop = true;
            bool canSpawnBottom = true;

            if (doorLocations.Count > 0)
            {
                foreach(Vector2 location in doorLocations)
                {
                    //this check where the door is located
                    if(location.x <= 0 || location.x >= lengthX)
                    {
                        //the door is either on the left or right.
                        if (Mathf.Abs(location.y - randY) < minDistBetweenDoors)
                        {
                            if (location.x == -2) canSpawnLeft = false;
                            else canSpawnRight = false;
                        }
                    }
                    else if (location.y <= 0 || location.y >= lengthY)
                    {
                        //the door is either on the top or bottom.
                        if (Mathf.Abs(location.x - randX) < minDistBetweenDoors)
                        {
                            if (location.y == -2) canSpawnTop = false;
                            else canSpawnBottom = false;
                        }
                    }
                }
            }

            List<int> spawnableLocations = new List<int>();

            if (canSpawnTop) spawnableLocations.Add(1);
            if (canSpawnLeft) spawnableLocations.Add(2);
            if (canSpawnRight) spawnableLocations.Add(3);
            if (canSpawnBottom) spawnableLocations.Add(4);

            if(spawnableLocations.Count > 0)
            {
                int index = Random.Range(0, spawnableLocations.Count);

                int location = spawnableLocations[index];

                if(location == 1)
                {
                    doorLocations.Add(new Vector2(randX, -2));
                }
                else if(location == 2)
                {
                    doorLocations.Add(new Vector2(-2, randY));
                }
                else if (location == 3)
                {
                    doorLocations.Add(new Vector2(lengthX, randY));
                }
                else if (location == 4)
                {
                    doorLocations.Add(new Vector2(randX, lengthY));
                }
            }
            else
            {
                i--;
                return;
            }
        }
    }

    private void GenerateDoors(GameObject wallTileParent, float lengthX, float lengthY)
    {
        foreach(Vector2 loc in doorLocations)
        {
            GameObject Doorway;
            float multiplierX = tileSize;
            float multiplierY = -tileSize;

            if (loc.x == -2)
            {
                Doorway = doorways[1];
            }
            else if (loc.x == lengthX)
            {
                Doorway = doorways[2];
            }
            else if (loc.y == -2)
            {
                Doorway = doorways[0];
            }
            else
            {
                Doorway = doorways[3];
            }

            Doorway.name = "Doorway_" + loc.x + "_" + loc.y;

            Doorway.transform.position = new Vector3(wallTileParent.transform.position.x + (loc.x * multiplierX), wallTileParent.transform.position.y + (loc.y * multiplierY), 0);

            GameObject.Instantiate(Doorway, Doorway.transform.position, Quaternion.identity, wallTileParent.transform);
        }
    }

    private bool CheckIfDoorLocation(int x, int y, float lengthX, float lengthY)
    {
        foreach(Vector2 loc in doorLocations)
        {
            if(loc.x <= 0 || loc.x >= lengthX) //the door is on the left of right
            {
                if(x <= 0 || x >= lengthX) //The tile is also on one of these sides
                {
                    if(x == loc.x) 
                    {
                        if (y >= 2 && y < lengthY - 1) //The tile we check is on the Y axis somewhere
                        {
                            if ((loc.y - y) <= 0 && (loc.y - y) > -3)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            else if(loc.y <= 0 || loc.y >= lengthY) //the door is on the top or bottom
            {
                if (y <= 0 || y >= lengthY) //The tile is also on one of these sides
                {
                    if(y == loc.y)
                    {
                        if (x >= 2 && x < lengthX - 1) //The tile we check is on the X axis somewhere
                        {
                            if ((loc.x - x) <= 0 && (loc.x - x) > -3)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public void GenerateWalls(Vector3 startPoint, float lengthX, float lengthY)
    {
        if (startPoint != null && lengthX > 0 && lengthY > 0)
        {
            bool[] cornerPlaced = new bool[5];
            bool[] doorwayPlaced = new bool[amountOfDoors + 1];

            GameObject WallTileParent = new GameObject { name = "WallTileParent" };
            WallTileParent.transform.position = startPoint;

            GenerateDoors(WallTileParent, lengthX, lengthY);

            for (int x = -2; x < lengthX +2; x++)
            {
                for(int y = -2; y < lengthY + 2; y++)
                {
                    if (!CheckIfDoorLocation(x, y, lengthX, lengthY))
                    {
                        GameObject WallTile;
                        float multiplierX = tileSize;
                        float multiplierY = -tileSize;

                        if (x < 0) //left walls
                        {
                            if (!cornerPlaced[0])
                            {
                                WallTile = wall_top_left_corner;
                                cornerPlaced[0] = true;
                            }
                            else
                            {
                                if (!cornerPlaced[3] && y >= lengthY)
                                {
                                    WallTile = wall_bottom_left_corner;
                                    cornerPlaced[3] = true;
                                }
                                else if (y >= 0 && x < -1 && y < lengthY)
                                {
                                    int rand = Random.Range(0, 101);

                                    if (rand <= 50)
                                    {
                                        WallTile = leftWalls[0];
                                    }
                                    else
                                    {
                                        WallTile = leftWalls[1];
                                    }
                                }
                                else
                                {
                                    WallTile = null;
                                }
                            }
                        }
                        else if (x >= (lengthX)) //right walls
                        {
                            if (!cornerPlaced[1])
                            {
                                WallTile = wall_top_right_corner;
                                cornerPlaced[1] = true;
                            }
                            else
                            {
                                if (!cornerPlaced[4] && y >= lengthY)
                                {
                                    WallTile = wall_bottom_right_corner;
                                    cornerPlaced[4] = true;
                                }
                                else if (y >= 0 && x < (lengthX + 1) && y < lengthY)
                                {
                                    int rand = Random.Range(0, 101);

                                    if (rand <= 50)
                                    {
                                        WallTile = rightWalls[0];
                                    }
                                    else
                                    {
                                        WallTile = rightWalls[1];
                                    }
                                }
                                else
                                {
                                    WallTile = null;
                                }
                            }
                        }
                        else if (y < -1)
                        {
                            int rand = Random.Range(0, 101);

                            if (rand <= 50)
                            {
                                WallTile = topWalls[0];
                            }
                            else
                            {
                                WallTile = topWalls[1];
                            }
                        }
                        else if (y == lengthY)
                        {
                            int rand = Random.Range(0, 101);

                            if (rand <= 50)
                            {
                                WallTile = bottomWalls[0];
                            }
                            else
                            {
                                WallTile = bottomWalls[1];
                            }
                        }
                        else
                        {
                            WallTile = null;
                        }


                        if (WallTile != null)
                        {
                            WallTile.name = "WallTile_" + x + "_" + y;

                            //prevent the hierarchy from becoming a mess

                            WallTile.transform.position = new Vector3(WallTileParent.transform.position.x + (x * multiplierX), WallTileParent.transform.position.y + (y * multiplierY), 0);

                            GameObject.Instantiate(WallTile, WallTile.transform.position, Quaternion.identity, WallTileParent.transform);
                        }
                    }
                }
            }
        }
    }

    public void GenerateFloor(Vector3 startPoint, GenerationDirection dir, FloorType type, float amountX, float amountY)
    {
        if(startPoint != null && amountX > 0 && amountY > 0)
        {
            float multiplierX = 0;
            float multiplierY = 0;

            switch (dir)
            {
                case GenerationDirection.BOTTOM_LEFT:
                        multiplierX = -tileSize;
                        multiplierY = -tileSize;
                    break;
                case GenerationDirection.BOTTOM_RIGHT:
                        multiplierX = tileSize;
                        multiplierY = -tileSize;
                    break;
                case GenerationDirection.TOP_LEFT:
                        multiplierX = -tileSize;
                        multiplierY = tileSize;
                    break;
                case GenerationDirection.TOP_RIGHT:
                        multiplierX = tileSize;
                        multiplierY = tileSize;
                    break;
                case GenerationDirection.NONE:
                        //cant do a generation to none, so default it to bottom right
                        multiplierX = tileSize;
                        multiplierY = -tileSize;
                    break;
            }

            GameObject FloorTileParent = new GameObject { name = "FloorTileParent"};

            FloorTileParent.transform.position = startPoint;

            for(int x = 0; x < amountX; x++)
            {
                for (int y = 0; y < amountY; y++)
                {
                    GameObject FloorTile;

                    float rand = Random.Range(0,101);

                    switch (type)
                    {
                        case FloorType.FLOOR_DIAMOND:

                            if (rand < 85f)
                            {
                                FloorTile = floorTiles[0];
                            }
                            else
                            {
                                FloorTile = floorTiles[1];
                            }

                            break;
                        case FloorType.FLOOR_BLANK_1:

                            if (rand < 85f)
                            {
                                FloorTile = floorTiles[2];
                            }
                            else
                            {
                                FloorTile = floorTiles[3];
                            }
                            break;
                        case FloorType.FLOOR_BLANK_2:

                            if (rand < 85f)
                            {
                                FloorTile = floorTiles[2];
                            }
                            else
                            {
                                FloorTile = floorTiles[3];
                            }
                            break;
                        default:
                            if (rand < 85f)
                            {
                                FloorTile = floorTiles[2];
                            }
                            else
                            {
                                FloorTile = floorTiles[3];
                            }
                            break;
                    }

                    FloorTile.name = "FloorTile_" + x + "_" + y;

                    //prevent the hierarchy from becoming a mess

                    FloorTile.transform.position = new Vector3(FloorTileParent.transform.position.x + (x * multiplierX), FloorTileParent.transform.position.y + (y * multiplierY), 0);

                    GameObject.Instantiate(FloorTile, FloorTile.transform.position, Quaternion.identity, FloorTileParent.transform);
                }
            }
        }
    }

    public List<EnemyController> GenerateEnemies(float lengthX, float lengthY, Vector2 playerLocation)
    {
        List<EnemyController> tempEnemyList = new List<EnemyController>();

        GameObject EnemyParent = new GameObject { name = "EnemyParent" };
        EnemyParent.transform.position = Vector3.zero;

        for (int i = 0; i < amountOfEnemies; i++)
        {
            int randX = (int)Random.Range(3, (lengthX - 3));
            int randY = (int)Random.Range(3, (lengthY - 3));

            bool canSpawnOnPickedLocation = true;

            if (Mathf.Abs(playerLocation.x - randX) < (minDistanceBetweenEnemies/2) && Mathf.Abs(playerLocation.y - -randY) < (minDistanceBetweenEnemies / 2))
            {
                canSpawnOnPickedLocation = false;
            }

            if (tempEnemyList.Count > 0)
            {
                foreach (EnemyController enemy in tempEnemyList)
                {
                    if (Mathf.Abs(enemy.enemyLocation.x - randX) < minDistanceBetweenEnemies && Mathf.Abs(enemy.enemyLocation.y - randY) < minDistanceBetweenEnemies)
                    {
                        canSpawnOnPickedLocation = false;
                    }
                }
            }

            if (canSpawnOnPickedLocation)
            {
                int rand = Random.Range(0, enemyPrefabs.Count);

                GameObject tempEnemy = enemyPrefabs[rand];
                tempEnemy.name = "Enemy_" + i;

                EnemyController enemyCont = tempEnemy.GetComponent<EnemyController>();
                enemyCont.enemyLocation = new Vector2(randX,randY);

                tempEnemy.transform.position = new Vector3((randX * 1.28f), (randY * -1.28f), 0);

                GameObject objToAdd = GameObject.Instantiate(tempEnemy, tempEnemy.transform.position, Quaternion.identity, EnemyParent.transform);

                tempEnemyList.Add(objToAdd.GetComponent<EnemyController>());
            }
            else
            {
                i--;
            }
        }

        return tempEnemyList;
    }
}
