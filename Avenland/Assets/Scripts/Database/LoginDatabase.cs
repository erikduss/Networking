using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class LoginDatabase : MonoBehaviour
{
    private static LoginDatabase _instance;
    public static LoginDatabase instance { get { return _instance; } }

    [Header("Pages")]
    public TMP_Text username;
    public GameObject registerPage;
    public GameObject loginPage;
    

    [Header("Login")]
    public TMP_InputField loginName;
    public TMP_InputField loginPassword;
    //public UserOnBoard yourScore;
    public TMP_Text infoTextLogin;

    [Header("Register")]
    public TMP_InputField registerName;
    public TMP_InputField registerPassword;
    public TMP_Text infoTextRegister;

    [Header("Logged in User")]
    public string LoggedInUsername;
    

    void Awake()
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

        DontDestroyOnLoad(this.gameObject);
    }

    #region registration
    public void Button_Register()
    {
        StartCoroutine(Register());
    }

    private void RegisterSuccess()
    {
        registerPage.SetActive(false);
        loginPage.SetActive(true);
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", registerName.text);
        form.AddField("pw", registerPassword.text);

        Debug.Log("Sending:: " + registerName.text + " _ " + registerPassword.text);

        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~erik.dusschooten/Homework/Jaar_2_Kernmodule_4/DatabaseConnectie/Register.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log(www.error);
            infoTextRegister.text = www.error;
        }
        else if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
            infoTextRegister.text = www.error;
        }
        else
        {
            Debug.Log(www.downloadHandler.text);

            if (www.downloadHandler.text[0] == '1')
            {
                string phrase = www.downloadHandler.text;
                Debug.Log(phrase);

                int pFrom = phrase.IndexOf("(Session");
                int pTo = phrase.LastIndexOf(")");
                string sessionID = phrase.Substring(pFrom, (pTo - pFrom) + 1);

                phrase = phrase.Replace(sessionID, "");
                phrase = phrase.Replace("<br>", "");

                infoTextRegister.text = phrase;

                if (phrase == "11")
                {
                    infoTextRegister.text = "Completed Registration";
                    RegisterSuccess();
                }
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                infoTextRegister.text = www.downloadHandler.text;
            }
        }
    }
    #endregion


    #region Login

    public void Button_Login()
    {
        StartCoroutine(Login());
    }

    private void LoginSuccess()
    {
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", loginName.text);
        form.AddField("pw", loginPassword.text);

        UnityWebRequest www = UnityWebRequest.Post("https://studenthome.hku.nl/~erik.dusschooten/Homework/Jaar_2_Kernmodule_4/DatabaseConnectie/user_login.php", form);
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError)
        {
            infoTextLogin.text = www.error;
        }
        else if (www.downloadHandler.text[0] == '1')
        {
            Debug.Log(www.downloadHandler.text);
            string phrase = www.downloadHandler.text;
            int pFrom = phrase.IndexOf("(Session");
            int pTo = phrase.LastIndexOf(")");
            string sessionID = phrase.Substring(pFrom, (pTo - pFrom)+1);

            phrase = phrase.Replace(sessionID, "");
            phrase = phrase.Replace("<br>", "");

            infoTextLogin.text = phrase;

            if (phrase[1] == '1')
            {
                infoTextLogin.text = $"Logged in as {loginName.text}!";
                //username.text = loginName.text;

                //string playerscore = phrase.Remove(0, 2);

                //yourScore.playerName.text = username.text;
                //playerscore = playerscore.Replace((username.text + " Highscore:"), "");
                //playerscore = playerscore.Replace("hello", "");
                //yourScore.playerScore.text = playerscore;

                LoggedInUsername = loginName.text;
                LoginSuccess();
            }
        }
        else
        {
            infoTextLogin.text = www.downloadHandler.text;
        }
    }

    #endregion
}
