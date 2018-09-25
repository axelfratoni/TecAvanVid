using System.Collections.Generic;
using System.Net;
using Events;
using Events.Actions;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isServer = true;
    
    public int _serverPort = 10000;
    public int _clientPort = 10001;
    public string serverIP = "127.0.0.1";

    private EventManager _eventManager;

    private void Start()
    {
        if (isServer)
        {
            _eventManager = new EventManager(this, _serverPort);
        }
        else
        {
            _eventManager = new EventManager(this, _clientPort);
            _eventManager.ConnectToServer(new IPEndPoint(IPAddress.Parse(serverIP), _serverPort));
        }
    }

    // This runs after connecting to the server.
    public void InitializeGame()
    {
        Debug.Log("Initializing game.");
        Debug.Log("Sending color action.");
        _eventManager.SendEventAction(new ColorAction(), 1);
    }

    public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
    {
    }

    public void ProcessColorAction()
    {
        Debug.Log("Color action in server.");
    }

    private void OnDisable()
    {
        _eventManager.Disable();
    }
}