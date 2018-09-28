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
    private BehaviourManager _behaviourManager;

    private void Start()
    {
        if (isServer)
        {
            _isConnected = true;
            _eventManager = new EventManager(this, _serverPort);
            _behaviourManager = new ServerManager();
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
        
        _behaviourManager = new ClientManager(_eventManager);
        
        Debug.Log("Initializing game.");
    }

    private void Update()
    {
        if (_behaviourManager != null)
        {
            _behaviourManager.Update();
        }
    }

    public void ProcessInput(double time, InputEnum input)
    {
        Debug.Log("Received input: " + input);
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