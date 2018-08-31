using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour{

    private Button submitButton;
    private GameObject gameManager;

    public InputField targetIP;
    public InputField targetPort;
    public InputField hostingPort;

    private void Start()
    {
        submitButton = GetComponent<Button>();
        gameManager = GameObject.Find("GameManager");
        submitButton.onClick.AddListener(HandleClick);
    }

    private void HandleClick()
    {
        if(hostingPort.isActiveAndEnabled)
        {
            object[] messageContent = new object[1];
            messageContent[0] = Int32.Parse(hostingPort.text);
            gameManager.SendMessage("InitializeServer", messageContent);
        }
        else
        {
            object[] messageContent = new object[2];
            messageContent[0] = targetIP.text;
            messageContent[1] = Int32.Parse(targetPort.text);
            gameManager.SendMessage("InitializeClient", messageContent);
        }

    }
}
