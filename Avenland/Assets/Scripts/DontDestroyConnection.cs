using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DontDestroyConnection : MonoBehaviour
{
    private static DontDestroyConnection _instance;
    public static DontDestroyConnection instance { get { return _instance; } }

    private SetSeed seedSetter;

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

        if(scene.name == "GameScene")
        {
            GameSettings.instance.SetUpGameScene();
        }
    }

    public void SwitchToScene(int sceneID, int seed)
    {


        Debug.Log("The seed is:: " + seed);
        seedSetter.SetNewSeed(seed);
        SceneManager.LoadScene(sceneID);
    }
}
