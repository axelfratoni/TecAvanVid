using System.Collections.Generic;
using Controllers;
using Events.Actions;
using Network;
using ShooterGame.Controllers;
using UnityEngine;

namespace Events
{
    public class ServerManager : MonoBehaviour
    {
        public GameObject ballPrefab;
        public GameObject projectilePrefab;
        public int serverPort = 10000;

        private ObjectFactory _objectFactory;
        private EventManager _eventManager;
        
        private readonly List<BallController> _balls = new List<BallController>();
        private readonly List<ProjectileController> _proyectiles = new List<ProjectileController>();
        
        private void Start()
        {
            _objectFactory = new ObjectFactory();
            _eventManager = new EventManager(serverPort, null);
        }
        
        public void Update()
        {
            Queue<Event> pendingEvents = _eventManager.GetPendingEvents();
            while (pendingEvents.Count > 0)
            {
                Event iEvent = pendingEvents.Dequeue();
                switch (iEvent.GetEventEnum())
                {
                    case EventEnum.CreationRequest:
                        ((CreationRequestAction)iEvent.GetPayload()).Extract(ProcessCreationRequest, iEvent.ClientId);
                        break;
                    case EventEnum.Movement:
                        //((MovementAction)iEvent.GetPayload()).Extract(ProcessInput, iEvent.ClientId);
                        break;
                }
            }
            
            _balls.ForEach(ball =>
            {
                Ball ballData = ball.GetBall();
                _eventManager.BroadcastEventAction(new SnapshotAction(ballData.ObjectId, ballData.Position, new Quaternion(), 0));
            });
        }
        
        private void ProcessInput(double time, double mouseX, List<InputEnum> inputList, int clientId) 
        {
            Debug.Log("Received input");
            BallController ballController = _balls.Find(ball => ball.GetBall().ClientId.Equals(clientId));
            if (ballController != null)
            {
                ballController.ApplyInput(time, inputList);
                /*if (inputList.Contains(InputEnum.ClickLeft))
                {
                    Projectile newProjectile =
                        _objectFactory.CreateProjectile(ballController.GetBall().Position, clientId, false);
                    _projectileCreationRequests.Enqueue(newProjectile);
                }*/
            }
        }

        public void ProcessCreationRequest(int clientId, Vector3 creationPosition, ObjectEnum objectType)
        {
            Debug.Log("Creation request received from client " + clientId + " position " + creationPosition);
            if (objectType.Equals(ObjectEnum.Ball))
            {
                Ball newBall = _objectFactory.CreateBall(creationPosition, clientId);
                _eventManager.BroadcastEventAction(new CreationAction(newBall.Position, newBall.ObjectId, ObjectEnum.Ball));
                _eventManager.SendEventAction(new AssignPlayerAction(newBall.ObjectId), clientId);                
                BallController ballController = Instantiate(ballPrefab).GetComponent<BallController>();
                ballController.SetBall(newBall);
                _balls.Add(ballController);
            }
        }
        
        private void OnDisable()
        {
            _eventManager.Disable();
        }
    }
}

public class ObjectFactory
{
    private int _objectId;
    
    public ObjectFactory()
    {
        _objectId = 0;
    }

    public Ball CreateBall(Vector3 position, int clientId)
    {
        Ball newBall = new Ball(clientId, _objectId++, position);
        return newBall;
    }

    public Projectile CreateProjectile(Vector3 position, int clientId, bool isControlled)
    {
        Projectile newProjectile = new Projectile(clientId, _objectId++, position, isControlled);
        return newProjectile;
    }
}