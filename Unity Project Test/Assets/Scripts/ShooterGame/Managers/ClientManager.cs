using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Timers;
using Events;
using Events.Actions;
using Libs;
using ShooterGame.Camera;
using ShooterGame.Controllers;
using ShooterGame.Controllers.Projectile;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Event = Events.Event;

namespace ShooterGame.Managers
{
    public class ClientManager : MonoBehaviour
    {
        public GameObject PlayerPrefab;
        public GameObject ProjectilePrefab;
        public GameObject Camera;
        public int ServerPort = 10000;
        public int ClientPort = 10001;
        public string ServerIP = "127.0.0.1";
        public bool Prediction;
        public int Delay;
        public float PacketLoss;

        private EventManager _eventManager;
        private bool _isConnected;
        private int _serverId;
        private List<ObjectController> _objects = new List<ObjectController>();
        private int _playerObjectId;
        private readonly TimeManager _timeManager = new TimeManager(SnapshotAction.MaxCycleTime);
        private Dictionary<InputEnum, bool> _lastInputSent = new Dictionary<InputEnum, bool>();
        
        private void Start()
        {
            _eventManager = new EventManager(ClientPort, InitializeGame, Delay, PacketLoss);
            _eventManager.ConnectToServer(new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort));
        }
        
         public void Update()
        {
            _timeManager.UpdateTime(Time.deltaTime);
            HandlePendingEvents();
            HandleInput();
        }

        private void HandlePendingEvents()
        {
            Queue<Event> pendingEvents = _eventManager.GetPendingEvents();
            while (pendingEvents.Count > 0)
            {
                Event iEvent = pendingEvents.Dequeue();
                if (iEvent == null) continue;
                switch (iEvent.GetEventEnum())
                {
                    case EventEnum.Snapshot:
                        ProcessSnapshot((SnapshotAction)iEvent.GetPayload());
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
        }

        private void HandleInput()
        {
            Dictionary<InputEnum, bool> inputMap = InputMapper.ExtractInput();
            float mouseX = Input.GetAxisRaw("Mouse X");
            float timeStamp = _timeManager.GetCurrentTime();
            bool isSameInputAsBefore = InputMapper.CompareInputs(inputMap, _lastInputSent);
            if (!isSameInputAsBefore)
            {
                //Debug.Log("Send input" + InputMapper.InputMapToInt(inputMap));
                _eventManager.SendEventAction(new MovementAction(timeStamp, mouseX, inputMap), _serverId);
                _lastInputSent = inputMap;
                
            }
            
            ObjectController playerController = _objects.Find(obj => obj.ObjectId.Equals(_playerObjectId));
            if (playerController != null && playerController.ObjectType.Equals(ObjectEnum.Player))
            {
                if (Math.Abs(mouseX) > 0.01f)
                {
                    ((PlayerController)playerController).ApplyMouseInput(mouseX);
                    Quaternion rotation = ((PlayerController) playerController).GetRotation();
                    _eventManager.SendEventAction(new RotationAction(rotation), _serverId);
                }
                if (Prediction)
                {
                    ((PlayerController)playerController).ApplyInputPrediction(inputMap, mouseX, timeStamp);
                }
            }
        }
        
        private void InitializeGame(int serverId)
        {
            if(_isConnected) return; 
            _isConnected = true;

            _serverId = serverId;
            _eventManager.SendEventAction(new CreationRequestAction(new Vector3(0, (float) 0.3, 0), ObjectEnum.Player), serverId);
        }
        
        private void ProcessSnapshot(SnapshotAction snapshotAction)
        {
            //Debug.Log("Received Snapshot");
            int objectId = snapshotAction.ObjectId;
            double timeStamp = snapshotAction.TimeStamp;
            Vector3 objectPosition = snapshotAction.ObjectPosition;
            Quaternion rotation = snapshotAction.ObjectRotation;
            double lastAckInputTime = snapshotAction.LastClientInputTime;
            
            
            ObjectController objectController = _objects.Find(obj => obj.ObjectId.Equals(objectId));
            if (objectController != null)
            {
                switch (objectController.ObjectType)
                {
                    case ObjectEnum.Player:
                        if (objectController.ObjectId.Equals(_playerObjectId))
                        {
                            if (Prediction)
                            {
                                ((PlayerController)objectController).ApplySnapshotPrediction(timeStamp, objectPosition + new Vector3(1,1,1), rotation, lastAckInputTime);
                            }
                            else
                            {
                                ((PlayerController)objectController).ApplySnapshot(timeStamp, objectPosition + new Vector3(1,1,1));
                            }
                        }
                        else
                        {
                            ((PlayerController)objectController).ApplySnapshot(timeStamp, objectPosition + new Vector3(1,1,1), rotation);
                        }
                        break;
                    case ObjectEnum.Projectile:
                        ((ProjectileController)objectController).ApplySnapshot(timeStamp, objectPosition + new Vector3(1,0,1));
                        break;
                }
            }
        }
        
        private void ProcessObjectCreation(int objectId, Vector3 creationPosition, ObjectEnum objectType, int clientId)
        {
            Debug.Log("Creation action received from server.");
            switch (objectType)
            {
                case ObjectEnum.Player:
                    PlayerController playerController = Instantiate(PlayerPrefab).GetComponent<PlayerController>();
                    bool isPlayer = objectId.Equals(_playerObjectId);
                    playerController.InitializeClient(objectId, clientId, creationPosition, Prediction && isPlayer);
                    _objects.Add(playerController);
                
                    if(isPlayer) Camera.GetComponent<CameraController>().SetPlayer(playerController.gameObject);    
                    break;
                
                case ObjectEnum.Projectile:
                    ProjectileController projectileController = Instantiate(ProjectilePrefab).GetComponent<ProjectileController>();
                    projectileController.InitializeClient(objectId, clientId, creationPosition + new Vector3(1,1,1));
                    _objects.Add(projectileController);
                    break;
            }
        }

        private void ProcessAssignPlayerAction(int objectId)
        {
            _playerObjectId = objectId;
            ObjectController playerController = _objects.Find(player => player.ObjectId.Equals(objectId));
            if (playerController != null && playerController.ObjectType.Equals(ObjectEnum.Player))
            {
                Camera.GetComponent<CameraController>().SetPlayer(playerController.gameObject);
                ((PlayerController)playerController).SetUpComponents(true, Prediction);
            }
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
            ObjectController objectCtrlr = _objects.Find(ply => ply.ObjectId.Equals(objectId));
            if (objectCtrlr != null)
            {
                switch (action)
                {
                    case SpecialActionEnum.FiringStart:
                        if(objectCtrlr.ObjectType.Equals(ObjectEnum.Player))
                            ((PlayerController)objectCtrlr).SetFiring(true);                        
                        break;
                    case SpecialActionEnum.FiringStop:
                        if(objectCtrlr.ObjectType.Equals(ObjectEnum.Player))
                            ((PlayerController)objectCtrlr).SetFiring(false);  
                        break;
                    case SpecialActionEnum.Destroy:
                        if (objectCtrlr.ObjectType.Equals(ObjectEnum.Projectile))
                        {
                            ((ProjectileController)objectCtrlr).Explode();
                            _objects = _objects.Where(obj => !obj.ObjectId.Equals(objectCtrlr.ObjectId)).ToList();
                        }
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