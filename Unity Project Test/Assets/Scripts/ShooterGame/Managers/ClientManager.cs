using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Timers;
using Events;
using Events.Actions;
using ShooterGame.Camera;
using ShooterGame.Controllers;
using UnityEngine;
using Debug = UnityEngine.Debug;
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
        private List<ObjectController> _objects = new List<ObjectController>();
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
                    case EventEnum.ReceiveDamage:
                        ((DamageAction)iEvent.GetPayload()).Extract(ProcessHealthUpdate, iEvent.ClientId);
                        break;
                }
            }
            
            Dictionary<InputEnum, bool> inputMap = InputMapper.ExtractInput();
            float mouseX = Input.GetAxisRaw("Mouse X");
            if (inputMap.Count > 0 || Math.Abs(mouseX) > 0.01)
            {
                _eventManager.SendEventAction(new MovementAction(0, mouseX, inputMap), _serverId);
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
            ObjectController playerCont = _objects.Find(obj => obj.ObjectId.Equals(objectId));
            if (playerCont != null)
            {
                switch (playerCont.ObjectType)
                {
                    case ObjectEnum.Player:
                        ((PlayerController)playerCont).ApplySnapshot(timeStamp, objectPosition + new Vector3(1,1,1), rotation);
                        break;
                }
            }
        }
        
        private void ProcessObjectCreation(int objectId, Vector3 creationPosition, ObjectEnum objectType, int clientId)
        {
            Debug.Log("Creation action received from server.");
            if (objectType.Equals(ObjectEnum.Player))
            {
                PlayerController playerController = Instantiate(PlayerPrefab).GetComponent<PlayerController>();
                playerController.Initialize(objectId, clientId, creationPosition);
                playerController.ToggleInputSnapshotController(false);
                playerController.SetShootableLayer(false);
                _objects.Add(playerController);
                
                if(objectId.Equals(_playerObjectId)) Camera.GetComponent<CameraController>().SetPlayer(playerController.gameObject);
            }
        }

        private void ProcessAssignPlayerAction(int objectId)
        {
            _playerObjectId = objectId;
            ObjectController playerController = _objects.Find(player => player.ObjectId.Equals(objectId));
            if(playerController != null) Camera.GetComponent<CameraController>().SetPlayer(playerController.gameObject);
        }

        private void ProcessHealthUpdate(int currentHealth, int objectId)
        {
            PlayerController playerController = (PlayerController) _objects.Find(player => player.ObjectId.Equals(objectId) &&
                                                                                           player.ObjectType.Equals(ObjectEnum.Player));
            if (playerController != null)
            {
                playerController.UpdateHealth(currentHealth);
                if (currentHealth == 0)
                {
                    Death(playerController);
                }
            }
        }

        private void Death(PlayerController playerController)
        {
            _objects = _objects.Where(player => !player.ObjectId.Equals(playerController.ObjectId)).ToList();

            if (playerController.ObjectId.Equals(_playerObjectId))
            {
                Camera.GetComponent<CameraController>().PlayerDeath();
                Timer aTimer = new Timer();
                aTimer.Elapsed += delegate
                {
                    aTimer.Dispose();
                    _isConnected = false;
                    InitializeGame(_serverId);
                };
                aTimer.Interval = 5000;
                aTimer.Enabled = true;
            }
            
        }

        private void ProcessSpecialAction(SpecialActionEnum action, int objectId)
        {
            Debug.Log("Received special action " + action);
            ObjectController objectController = _objects.Find(ply => ply.ObjectId.Equals(objectId));
            if (objectController != null)
            {
                switch (action)
                {
                    case SpecialActionEnum.FiringStart:
                        if(objectController.ObjectType.Equals(ObjectEnum.Player))
                            ((PlayerController)objectController).SetFiring(true);                        
                        break;
                    case SpecialActionEnum.FiringStop:
                        if(objectController.ObjectType.Equals(ObjectEnum.Player))
                            ((PlayerController)objectController).SetFiring(false);  
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