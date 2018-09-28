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

    private bool _isConnected;
    private EventManager _eventManager;

    private void Start()
    {
        if (isServer)
        {
            _isConnected = true;
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
        if(_isConnected) return; 
        _isConnected = true;
        
        Debug.Log("Initializing game.");
        Debug.Log("Sending color action.");
        _eventManager.SendEventAction(new ColorAction(45, _clientPort%300, 135), 1);
    }

    public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
    {
    }

    public void ProcessColorAction(int r, int g, int b)
    {
        Debug.Log("Received color: r " + r + " g " + g + " b " + b);
    }

    private void OnDisable()
    {
        _eventManager.Disable();
    }
}