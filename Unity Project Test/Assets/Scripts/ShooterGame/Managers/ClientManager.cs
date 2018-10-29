using System;
using System.Collections.Generic;
using System.Net;
using Events;
using Events.Actions;
using ShooterGame.Camera;
using ShooterGame.Controllers;
using UnityEngine;
using Event = Events.Event;

namespace ShooterGame.Managers
{
    public class ClientManager : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public GameObject Camera;
        public int ServerPort = 10000;
        public int ClientPort = 10001;
        public string ServerIP = "127.0.0.1";

        private EventManager _eventManager;
        private bool _isConnected;
        private int _serverId;
        private readonly List<PlayerController> _players = new List<PlayerController>();
        private int _playerObjectId;
        
        private void Start()
        {
            _eventManager = new EventManager(ClientPort, InitializeGame);
            _eventManager.ConnectToServer(new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort));
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
                    case EventEnum.Special:
                        ((SpecialAction)iEvent.GetPayload()).Extract(ProcessSpecialAction, iEvent.ClientId);
                        break;
                }
            }
            
            List<InputEnum> inputList = InputMapper.ExtractInput();
            float mouseX = Input.GetAxisRaw("Mouse X");
            if (inputList.Count > 0 || Math.Abs(mouseX) > 0.01)
            {
                _eventManager.SendEventAction(new MovementAction(0, mouseX, inputList), _serverId);
            }
        }
        
        private void InitializeGame(int serverId)
        {
            if(_isConnected) return; 
            _isConnected = true;

            _serverId = serverId;
            _eventManager.SendEventAction(new CreationRequestAction(new Vector3(0, (float) 0.3, 0), ObjectEnum.Player), serverId);
        }
        
        private void ProcessSnapshot(int objectId, Vector3 objectPosition, Quaternion rotation, double timeStamp)
        {
            Debug.Log("Received Snapshot");
            PlayerController playerCont = _players.Find(player => player.ObjectId.Equals(objectId));
            if(playerCont != null) playerCont.ApplySnapshot(timeStamp, objectPosition, rotation);
        }
        
        private void ProcessObjectCreation(int objectId, Vector3 creationPosition, ObjectEnum objectType, int clientId)
        {
            Debug.Log("Creation action received from server.");
            if (objectType.Equals(ObjectEnum.Player))
            {
                PlayerController playerController = Instantiate(PlayerPrefab).GetComponent<PlayerController>();
                playerController.Initialize(objectId, clientId, creationPosition);
                playerController.ToggleInputSnapshotController(false);
                _players.Add(playerController);
                
                if(objectId.Equals(_playerObjectId)) Camera.GetComponent<CameraController>().SetPlayer(playerController.gameObject);
            }
        }

        private void ProcessAssignPlayerAction(int objectId)
        {
            _playerObjectId = objectId;
            PlayerController playerController = _players.Find(player => player.ObjectId.Equals(objectId));
            if(playerController != null) Camera.GetComponent<CameraController>().SetPlayer(playerController.gameObject);
        }

        private void ProcessSpecialAction(SpecialActionEnum action, int objectId)
        {
            Debug.Log("Received special action " + action);
            PlayerController player = _players.Find(ply => ply.ObjectId.Equals(objectId));
            if (player != null)
            {
                switch (action)
                {
                    case SpecialActionEnum.FiringStart:
                        player.SetFiring(true);                        
                        break;
                    case SpecialActionEnum.FiringStop:
                        player.SetFiring(false);
                        break;
                }
            }

        }
        
        private void OnDisable()
        {
            _eventManager.Disable();
        }
    }
}