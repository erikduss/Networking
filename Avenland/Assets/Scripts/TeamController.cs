using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamController : MonoBehaviour
{
    private static TeamController _instance;
    public static TeamController instance { get { return _instance; } }

    private GameObject teamObject;
    private GameManager gameManager;

    public Vector2 playerLocation;

    public bool isPlayersTurn = false;

    private GameSettings settings;

    [SerializeField] private List<GameObject> specializationGameobjects;
    private Vector3 hostPosition = new Vector3(0,0,0);

    private List<Vector3> additionalPlayerPositions = new List<Vector3>();

    public List<NetworkedGamePlayer> players = new List<NetworkedGamePlayer>();
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
        if (!isPlayersTurn) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x, playerLocation.y + 1)))
            {
                teamObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1.28f, 0);
                playerLocation.y++;
                gameManager.EndPlayerTurn();
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x - 1, playerLocation.y)))
            {
                teamObject.transform.position = new Vector3(transform.position.x - 1.28f, transform.position.y, 0);
                playerLocation.x--;
                gameManager.EndPlayerTurn();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x + 1, playerLocation.y)))
            {
                teamObject.transform.position = new Vector3(transform.position.x + 1.28f, transform.position.y, 0);
                playerLocation.x++;
                gameManager.EndPlayerTurn();
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (IsMovementToLocationValid(new Vector2(playerLocation.x, playerLocation.y - 1)))
            {
                teamObject.transform.position = new Vector3(transform.position.x, transform.position.y - 1.28f, 0);
                playerLocation.y--;
                gameManager.EndPlayerTurn();
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

    public void UpdateTeam()
    {
        GameObject spec;

        foreach(NetworkedGamePlayer player in players)
        {
            if (player.isLocal)
            {
                spec = specializationGameobjects[(int)player.selectedSpecialization];
                spec.SetActive(true);
                spec.transform.localPosition = hostPosition;
                spec.GetComponent<SpriteRenderer>().sortingOrder = 4;

                UIManager.instance.SetPlayerHUD(0, player.selectedSpecialization, player.playerName);
            }
            else
            {
                amountOfExtraPlayers++;
                spec = specializationGameobjects[(int)player.selectedSpecialization];
                spec.SetActive(true);
                spec.transform.localPosition = additionalPlayerPositions[amountOfExtraPlayers-1];
                spec.GetComponent<SpriteRenderer>().sortingOrder = (4- amountOfExtraPlayers);

                UIManager.instance.SetPlayerHUD(amountOfExtraPlayers, player.selectedSpecialization, player.playerName);
            }
        }

        //for (int i = 0; i< settings.chosenSpecializations.Count; i++)
        //{
        //    switch (i)
        //    {
        //        case 0:
        //                spec = specializationGameobjects[((int)settings.chosenSpecializations[i])-1];
        //                spec.SetActive(true);
        //                spec.transform.localPosition = hostPosition;
        //                spec.GetComponent<SpriteRenderer>().sortingOrder = 4;
        //            break;
        //        case 1:
        //                spec = specializationGameobjects[((int)settings.chosenSpecializations[i])-1];
        //                spec.SetActive(true);
        //                spec.transform.localPosition = additionalPlayer_1_position;
        //                spec.GetComponent<SpriteRenderer>().sortingOrder = 3;
        //            break;
        //        case 2:
        //                spec = specializationGameobjects[((int)settings.chosenSpecializations[i])- 1];
        //                spec.SetActive(true);
        //                spec.transform.localPosition = additionalPlayer_1_position;
        //                spec.GetComponent<SpriteRenderer>().sortingOrder = 2;
        //            break;
        //        case 3:
        //                spec = specializationGameobjects[((int)settings.chosenSpecializations[i]) -1];
        //                spec.SetActive(true);
        //                spec.transform.localPosition = additionalPlayer_1_position;
        //                spec.GetComponent<SpriteRenderer>().sortingOrder = 1;
        //            break;
        //    }
        //}
    }
}
