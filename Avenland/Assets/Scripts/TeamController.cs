using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TeamMoveDirection
{
    NONE,
    TOP,
    RIGHT,
    DOWN,
    LEFT
}

public class TeamController : MonoBehaviour
{
    private static TeamController _instance;
    public static TeamController instance { get { return _instance; } }

    private GameObject teamObject;
    private GameManager gameManager;

    public Vector2 playerLocation;

    public bool isServer = false;
    public bool isPlayersTurn = false;
    private int currentPlayerTurn;

    private GameSettings settings;

    [SerializeField] private List<GameObject> specializationGameobjects;
    private Vector3 hostPosition = new Vector3(0,0,0);

    private List<Vector3> additionalPlayerPositions = new List<Vector3>();

    public List<NetworkedGamePlayer> players = new List<NetworkedGamePlayer>();
    private NetworkedGamePlayer localPlayer;
    private int amountOfExtraPlayers = 0;

    private void Awake()
    {
        //THERE CAN ONLY BE ONE INSTANCE OF THIS SCRIPT AT ONE TIME
        if (instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        teamObject = this.gameObject;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();

        settings = GameObject.FindGameObjectWithTag("GameOptions").GetComponent<GameSettings>();

        additionalPlayerPositions.Add(new Vector3(0.6f, 0.22f));
        additionalPlayerPositions.Add(new Vector3(0.6f, 0.22f));
        additionalPlayerPositions.Add(new Vector3(0.6f, 0.22f));
    }

    // Update is called once per frame
    void Update()
    {
        CheckInputs();
    }

    private void CheckInputs()
    {
        if (!isPlayersTurn || isServer) return;

        //only take the input, send to server to confirm if its valid and let the server send a message back to actually move.

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x, playerLocation.y + 1)))
            {
                EndPlayerTurn(TeamMoveDirection.TOP);
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x - 1, playerLocation.y)))
            {
                EndPlayerTurn(TeamMoveDirection.LEFT);
            }
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x + 1, playerLocation.y)))
            {
                EndPlayerTurn(TeamMoveDirection.RIGHT);
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x, playerLocation.y - 1)))
            {
                EndPlayerTurn(TeamMoveDirection.DOWN);
            }
        }
    }

    private bool IsMovementToLocationValid(Vector2 targetLocation)
    {
        if(targetLocation.x < 0 || targetLocation.x > (gameManager.dungeonSizeX-2) || targetLocation.y > 1 || targetLocation.y < -(gameManager.dungeonSizeY - 2))
        {
            Debug.Log("Cant move further -> hit the edge");
            return false;
        }

        return true;
    }

    //THIS FUNCTION SHOULD ONLY GET CALLED FROM A SERVER BROADCAST
    public void UpdateTeamPosition(TeamMoveDirection direction)
    {
        switch (direction)
        {
            case TeamMoveDirection.TOP:
                    teamObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.28f, 0);
                    playerLocation.y++;
                break;
            case TeamMoveDirection.RIGHT:
                    teamObject.transform.position = new Vector3(transform.position.x + 1.28f, transform.position.y, 0);
                    playerLocation.x++;
                break;
            case TeamMoveDirection.DOWN:
                    teamObject.transform.position = new Vector3(transform.position.x, transform.position.y - 1.28f, 0);
                    playerLocation.y--;
                break;
            case TeamMoveDirection.LEFT:
                    teamObject.transform.position = new Vector3(transform.position.x - 1.28f, transform.position.y, 0);
                    playerLocation.x--;
                break;
            case TeamMoveDirection.NONE:
                break;
        }

        HandleEnemyDetection();
    }

    private void HandleEnemyDetection()
    {
        foreach (EnemyController enemy in GameManager.instance.enemies)
        {
            enemy.LookForNearbyPlayer(playerLocation);
        }
    }

    public void UpdateTeam()
    {
        GameObject spec;
        amountOfExtraPlayers = 0;

        foreach(NetworkedGamePlayer player in players)
        {
            if (player.isLocal || amountOfExtraPlayers >= 4)//The server wont spawn a local
            {
                spec = specializationGameobjects[(int)player.selectedSpecialization-1];
                spec.SetActive(true);
                spec.transform.localPosition = hostPosition;
                spec.GetComponent<SpriteRenderer>().sortingOrder = 4;

                UIManager.instance.SetPlayerHUD(0, player.selectedSpecialization, player.playerName);
            }
            else
            {
                amountOfExtraPlayers++;
                spec = specializationGameobjects[(int)player.selectedSpecialization-1];
                spec.SetActive(true);
                spec.transform.localPosition = additionalPlayerPositions[amountOfExtraPlayers-1];
                spec.GetComponent<SpriteRenderer>().sortingOrder = (4- amountOfExtraPlayers);

                UIManager.instance.SetPlayerHUD(amountOfExtraPlayers, player.selectedSpecialization, player.playerName);
            }
        }
    }

    public void SetTurn(int turn)
    {
        currentPlayerTurn = turn;

        if (localPlayer == null) localPlayer = players.Where(a => a.isLocal == true).Single();

        if (currentPlayerTurn == localPlayer.networkId)
        {
            isPlayersTurn = true;
        }
        else
        {
            isPlayersTurn = false;
        }

        foreach(NetworkedGamePlayer player in players)
        {
            if (player.networkId != currentPlayerTurn)
            {
                player.isPlayersTurn = false;
            }
            else
            {
                player.isPlayersTurn = true;
            }
        }
    }

    public uint GetNextPlayerTurnID()
    {
        if (!isServer) return 100;

        int currentPlayerIndex = -1;

        for(int i = 0; i< players.Count; i++)
        {
            if (currentPlayerTurn == players[i].networkId) currentPlayerIndex = i;
        }

        uint IDToReturn;

        if(currentPlayerIndex == (players.Count - 1))
        {
            IDToReturn = players[0].networkId;
        }
        else
        {
            IDToReturn = players[currentPlayerIndex + 1].networkId;
        }

        return IDToReturn;
    }

    public void EndPlayerTurn(TeamMoveDirection direction)
    {
        isPlayersTurn = false; //ONLY 1 input.

        //This can only happen if it was your turn!
        //Send a message to the server to validate the move!

        EndTurnMessage etmsg = new EndTurnMessage
        {
            networkId = localPlayer.networkId,
            moveDirection = (uint)direction
        };

        Client.instance.SendPackedMessage(etmsg);
    }

    public bool ServerCheckNewPositionValid(TeamMoveDirection direction)
    {
        if (!isServer) return false;

        switch (direction)
        {
            case TeamMoveDirection.TOP:
                    if (IsMovementToLocationValid(new Vector2(playerLocation.x, playerLocation.y + 1)))
                    {
                        UpdateTeamPosition(direction);
                        return true;
                    }
                break;
            case TeamMoveDirection.RIGHT:
                    if (IsMovementToLocationValid(new Vector2(playerLocation.x + 1, playerLocation.y)))
                    {
                        UpdateTeamPosition(direction);
                        return true;
                    }
                break;
            case TeamMoveDirection.DOWN:
                    if (IsMovementToLocationValid(new Vector2(playerLocation.x, playerLocation.y - 1)))
                    {
                        UpdateTeamPosition(direction);
                        return true;
                    }
                break;
            case TeamMoveDirection.LEFT:
                    if (IsMovementToLocationValid(new Vector2(playerLocation.x - 1, playerLocation.y)))
                    {
                        UpdateTeamPosition(direction);
                        return true;
                    }
                break;
            case TeamMoveDirection.NONE:
                    return true;
        }

        return false;
    }
}
