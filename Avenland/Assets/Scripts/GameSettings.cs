using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    private static GameSettings _instance;
    public static GameSettings instance { get { return _instance; } }

    public uint localPlayerID;

    public bool isServerOperator;

    public int seed;
    public bool randomizeSeed;

    public DungeonSize dungeonSize;

    public int amountOfPlayers;

    public List<SpecializationType> chosenSpecializations;
    public List<string> playerNames;

    private LobbyManager lobbyManager;
    private UIManager uiManager;
    private NetworkManager networkManager;

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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //if(SceneManager.GetActiveScene().buildIndex != lastLoadedSceneID && isHost)
        //{
        //    UpdateServerInfo();
        //}
    }

    public bool ValidSettings()
    {
        if (amountOfPlayers < 1)
        {
            return false;
        }
        else return true;
    }

    public void GrandOperatorPower()
    {
        if (isServerOperator)
        {
            lobbyManager = FindObjectOfType<LobbyManager>();
            lobbyManager.SetOperatorStatus(true);
        }
    }

    public void TakeOperatorPower()
    {
        if(lobbyManager != null)
        {
            lobbyManager.SetOperatorStatus(false);
        }
    }

    public void HandleReadyStatusChanged()
    {
        if (isServerOperator)
        {
            lobbyManager.CheckReadyValidState();
        }
    }

    public void SetUpGameScene()
    {
        networkManager = GetComponent<NetworkManager>();
        uiManager = FindObjectOfType<UIManager>();

        Debug.Log(amountOfPlayers + " _ " + chosenSpecializations.Count + " _ " + playerNames.Count);

        //uiManager.SetPlayerHUD(amountOfPlayers, chosenSpecializations, playerNames);
    }
}
