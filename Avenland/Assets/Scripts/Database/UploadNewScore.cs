using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class UploadNewScore : MonoBehaviour
{
    [SerializeField] int currentPoints;
    [SerializeField] TMP_Text pointsText;

    public static UploadNewScore instance;

    public bool uploadedScore = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
        }
        instance = this;
    }

    public void AddPoints(int points)
    {
        currentPoints += points;
        //pointsText.text = "Score: " + currentPoints.ToString();
    }

    public int GetPoints()
    {
        return currentPoints;
    }

    public void UploadScore(int score)
    {
         StartCoroutine(UploadScoreToDatabase(LoginDatabase.instance.LoggedInUsername, score));
    }

    IEnumerator UploadScoreToDatabase(string username, int score)
    {
        uploadedScore = false;

        WWWForm form = new WWWForm();
        form.AddField("username", username);
        form.AddField("score", score);

        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~erik.dusschooten/Homework/Jaar_2_Kernmodule_4/DatabaseConnectie/insert_score.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
        }
        else
        {
            uploadedScore = true;
            Debug.Log(www.downloadHandler.text);
        }
        currentPoints = 0;
    }
}
