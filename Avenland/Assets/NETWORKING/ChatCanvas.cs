using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

namespace ChatClientExample
{
    public class ChatCanvas : MonoBehaviour
    {
        public static Color chatColor = new Color(0f, 0f, 0f); //old 0.85, 0.85, 0.85
        public static Color joinColor = new Color(0f, 1f, 0f); //old 0.25, 0.95, 0.25
        public static Color leaveColor = new Color(.95f, .25f, .25f);

        public GameObject textPrefab;
        public Transform chatPanel;

        public Queue<GameObject> textInstances = new Queue<GameObject>();

        public int maxMessages = 32;

        public void NewMessage(string message, Color color) {
            GameObject newInstance = GameObject.Instantiate(textPrefab);

            newInstance.GetComponent<Text>().text = $"{message}";
            newInstance.GetComponent<Text>().color = color;
            newInstance.transform.SetParent( chatPanel );

            newInstance.SetActive(true);

            if (textInstances.Count > maxMessages) {
                Destroy(textInstances.Dequeue());
            }

            textInstances.Enqueue(newInstance);
        }
    }
}