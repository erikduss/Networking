using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [SerializeField] private ClientBehaviour client;
    [SerializeField] private InputField inputNumber;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SendInt()
    {
        if (inputNumber.text.Length < 1) return;

        uint value = uint.Parse(inputNumber.text);

        client.SendInt(value);
    }

    public void SendString()
    {
        if (inputNumber.text.Length < 1) return;

        string value = inputNumber.text;

        client.SendString(value);
    }
}
