using System.Collections.Generic;
using System.Net;
using Controllers;
using Events.Actions;
using Network;
using UnityEngine;

namespace Events
{
    public class ClientManager : MonoBehaviour
    {
        public GameObject ballPrefab;
        public GameObject projectilePrefab;
        public GameObject Camera;
        public int _serverPort = 10000;
        public int _clientPort = 10001;
        public string serverIP = "127.0.0.1";

        private EventManager _eventManager;
        private ActionDispatcher _actionDispatcher;
        private bool _isConnected;
        private int _serverId;
        private readonly List<BallController> _balls = new List<BallController>();
        private readonly Queue<Ball> _creationRequests = new Queue<Ball>();
        private int _playerObjectId;

        private void Start()
        {
            _actionDispatcher = new ActionDispatcher(this);
            _eventManager = new EventManager(_actionDispatcher, _clientPort);
            _eventManager.ConnectToServer(new IPEndPoint(IPAddress.Parse(serverIP), _serverPort));
        }

        public void Update()
        {
            List<InputEnum> inputList = InputMapper.ExtractInput();
            if (inputList.Count > 0)
            {
                _eventManager.SendEventAction(new MovementAction(0, inputList), _serverId);
            }
            
            while (_creationRequests.Count > 0)
            {
                Ball ball = _creationRequests.Dequeue();
                BallController ballController = Instantiate(ballPrefab).GetComponent<BallController>();
                ballController.SetBall(ball);
                if(ball.ObjectId.Equals(_playerObjectId)) Camera.GetComponent<CameraController>().SetPlayer(ballController.gameObject);
                _balls.Add(ballController);
            }
        }
        
        public void InitializeGame(int serverId)
        {
            if(_isConnected) return; 
            _isConnected = true;

            _serverId = serverId;
            _eventManager.SendEventAction(new CreationRequestAction(new Vector3(0, (float) 0.3, 0), ObjectEnum.Ball), serverId);
        }
        
        public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
        {
            Debug.Log("Received Snapshot");
            BallController ballCont = _balls.Find(ball => ball.GetBall().ObjectId.Equals(objectId));
            if(ballCont != null) ballCont.ApplySnapshot(timeStamp, objectPosition + new Vector3(1,1,1));
        }
        
        public void ProcessObjectCreation(Vector3 creationPosition, int clientId, int objectId, ObjectEnum objectType)
        {
            Debug.Log("Creation action received from server.");
            if (objectType.Equals(ObjectEnum.Ball))
            {
                Ball newBall = new Ball(clientId, objectId, creationPosition);
                _creationRequests.Enqueue(newBall); 
            }
        }

        public void ProcessAssignPlayerAction(int objectId)
        {
            _playerObjectId = objectId;
            BallController ballCont = _balls.Find(ball => ball.GetBall().ObjectId.Equals(objectId));
            if(ballCont != null) Camera.GetComponent<CameraController>().SetPlayer(ballCont.gameObject);
        }
        
        private void OnDisable()
        {
            _eventManager.Disable();
        }
    }
}