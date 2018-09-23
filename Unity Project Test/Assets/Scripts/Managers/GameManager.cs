using System.Collections.Generic;
using System.Net;
using Events;
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

    public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
    {
        
    }

    private void OnDisable()
    {
        _eventManager.Disable();
    }
}