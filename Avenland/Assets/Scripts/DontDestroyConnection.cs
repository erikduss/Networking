using ChatClientExample;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyConnection : MonoBehaviour
{
    private static DontDestroyConnection _instance;
    public static DontDestroyConnection instance { get { return _instance; } }

    private SetSeed seedSetter;

    public bool isServer = false;

    void Awake()
    {
        //THERE CAN ONLY BE ONE INSTANCE OF THIS SCRIPT AT ONE TIME
        if (instance == null)
        {
            _instance = this;
            seedSetter = GetComponent<SetSeed>();
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded scene: " + scene.buildIndex);

        if (scene.buildIndex == 0) Destroy(gameObject);

        if (!isServer)
        {
            //Send to the server if we loaded into the correct scene
            LoadedGameMessage loadMessage;
            if (scene.name == "GameScene")
            {
                loadMessage = new LoadedGameMessage
                {
                    networkId = GameSettings.instance.localPlayerID,
                    status = 1
                };
                GameSettings.instance.SetUpGameScene();
            }
            else
            {
                loadMessage = new LoadedGameMessage
                {
                    networkId = GameSettings.instance.localPlayerID,
                    status = 0
                };
            }

            //Send a message to the server that we loaded into the game.
            Client.instance.SendPackedMessage(loadMessage);
        }
    }

    public void SwitchToScene(int sceneID, int seed)
    {
        seedSetter.SetNewSeed(seed);
        SceneManager.LoadScene(sceneID);
    }
}
