using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    private SceneManager sceneManager;

    private Slider loadingbar;
    public float progress = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator StartGameScene()
    {
        /*if (loadingbar == null)
        {
            loadingbar = GameObject.FindWithTag("loadingBar").GetComponent<Slider>();
        }*/
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GameScene");
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads

        progress = asyncLoad.progress;

        //loadingbar.value = asyncLoad.progress;


        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress);

            progress = asyncLoad.progress;

            if (asyncLoad.progress >= 0.9f)
            {
                //Change the Text to show the Scene is ready
                Debug.Log("Press the space bar to continue");
                //Wait to you press the space key to activate the Scene
                if (Input.GetKeyDown(KeyCode.Space))
                    //Activate the Scene
                    asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        if (asyncLoad.isDone)
        {
            progress = asyncLoad.progress;
            asyncLoad.allowSceneActivation = true;
            SceneManager.LoadScene("GameScene"); // Dont know why the async doesnt work, temporary load fix.
            Destroy(this.gameObject);
        }
    }

    private IEnumerator startLoading()
    {
        new WaitForSeconds(2);
        StartCoroutine("StartGameScene");
        return null;
    }
}
