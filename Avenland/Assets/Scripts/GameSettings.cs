using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSettings : MonoBehaviour
{
    public bool isServerOperator;

    public int seed;
    public bool randomizeSeed;

    public DungeonSize dungeonSize;

    public int amountOfPlayers;

    public List<SpecializationType> chosenSpecializations;
    public List<string> playerNames;

    private LobbyManager lobbyManager;

    // Start is called before the first frame update
    void Start()
    {
        
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

    private void UpdateServerInfo()
    {
        switch (SceneManager.GetActiveScene().buildIndex)
        {
            //case 0:
                
            //    break;
            //case 1:
            //        hostServer.chat = GameObject.FindGameObjectWithTag("ChatCanvas").GetComponent<ChatCanvas>();
            //    break;
            //case 2:
            //        hostServer.chat = GameObject.FindGameObjectWithTag("ChatCanvas").GetComponent<ChatCanvas>();
            //    break;
            //case 3:
            //        hostServer.chat = GameObject.FindGameObjectWithTag("ChatCanvas").GetComponent<ChatCanvas>();
            //    break;
        }
    }
}
