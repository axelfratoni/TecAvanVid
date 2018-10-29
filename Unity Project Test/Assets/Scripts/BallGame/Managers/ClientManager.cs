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
        private bool _isConnected;
        private int _serverId;
        private readonly List<BallController> _balls = new List<BallController>();
        private int _playerObjectId;

        private void Start()
        {
            _eventManager = new EventManager(_clientPort, InitializeGame);
            _eventManager.ConnectToServer(new IPEndPoint(IPAddress.Parse(serverIP), _serverPort));
        }

        public void Update()
        {
            Queue<Event> pendingEvents = _eventManager.GetPendingEvents();
            while (pendingEvents.Count > 0)
            {
                Event iEvent = pendingEvents.Dequeue();
                switch (iEvent.GetEventEnum())
                {
                    case EventEnum.Snapshot:
                        ((SnapshotAction)iEvent.GetPayload()).Extract(ProcessSnapshot, iEvent.ClientId);
                        break;
                    case EventEnum.AssignPlayer:
                        ((AssignPlayerAction)iEvent.GetPayload()).Extract(ProcessAssignPlayerAction, iEvent.ClientId);
                        break;
                    case EventEnum.Creation:
                        ((CreationAction)iEvent.GetPayload()).Extract(ProcessObjectCreation, iEvent.ClientId);
                        break;
                }
            }
            
            List<InputEnum> inputList = InputMapper.ExtractInput();
            if (inputList.Count > 0)
            {
                _eventManager.SendEventAction(new MovementAction(0,0, inputList), _serverId);
            }
            
            /*while (_creationRequests.Count > 0)
            {
                Ball ball = _creationRequests.Dequeue();
                BallController ballController = Instantiate(ballPrefab).GetComponent<BallController>();
                ballController.SetBall(ball);
                if(ball.ObjectId.Equals(_playerObjectId)) Camera.GetComponent<CameraController>().SetPlayer(ballController.gameObject);
                _balls.Add(ballController);
            }*/
        }
        
        public void InitializeGame(int serverId)
        {
            if(_isConnected) return; 
            _isConnected = true;

            _serverId = serverId;
            _eventManager.SendEventAction(new CreationRequestAction(new Vector3(0, (float) 0.3, 0), ObjectEnum.Ball), serverId);
        }
        
        public void ProcessSnapshot(int objectId, Vector3 objectPosition, Quaternion rotation, double timeStamp)
        {
            Debug.Log("Received Snapshot");
            BallController ballCont = _balls.Find(ball => ball.GetBall().ObjectId.Equals(objectId));
            if(ballCont != null) ballCont.ApplySnapshot(timeStamp, objectPosition + new Vector3(1,1,1));
        }
        
        public void ProcessObjectCreation(int objectId, Vector3 creationPosition, ObjectEnum objectType, int clientId)
        {
            Debug.Log("Creation action received from server.");
            if (objectType.Equals(ObjectEnum.Ball))
            {
                Ball newBall = new Ball(clientId, objectId, creationPosition);
                BallController ballController = Instantiate(ballPrefab).GetComponent<BallController>();
                ballController.SetBall(newBall);
                if(newBall.ObjectId.Equals(_playerObjectId)) Camera.GetComponent<CameraController>().SetPlayer(ballController.gameObject);
                _balls.Add(ballController);
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