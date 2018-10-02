using System.Collections.Generic;
using Events.Actions;
using Network;
using UnityEngine;

namespace Events
{
    public class ServerManager : MonoBehaviour
    {
        public GameObject ballPrefab;
        public GameObject proyectilePrefab;
        public int serverPort = 10000;

        private ObjectFactory _objectFactory;
        private EventManager _eventManager;
        private ActionDispatcher _actionDispatcher;
        private readonly List<BallController> _balls = new List<BallController>();
        private readonly Queue<Ball> _creationRequests = new Queue<Ball>();
        
        private void Start()
        {
            _objectFactory = new ObjectFactory();
            _actionDispatcher = new ActionDispatcher(this);
            _eventManager = new EventManager(_actionDispatcher, serverPort);
        }
        
        public void Update()
        {
            while (_creationRequests.Count > 0)
            {
                Ball ball = _creationRequests.Dequeue();
                BallController ballController = Instantiate(ballPrefab).GetComponent<BallController>();
                ballController.SetBall(ball);
                _balls.Add(ballController);
            }
            
            _balls.ForEach(ball =>
            {
                Ball ballData = ball.GetBall();
                _eventManager.BroadcastEventAction(new SnapshotAction(ballData.ObjectId, ballData.Position, 0));
            });
        }
        
        public void ProcessInput(double time, List<InputEnum> inputList, int clientId) 
        {
            Debug.Log("Received input");
            BallController ballController = _balls.Find(ball => ball.GetBall().ClientId.Equals(clientId));
            if(ballController != null) ballController.ApplyInput(time, inputList);
        }

        public void ProcessCreationRequest(int clientId, ObjectEnum objectType, Vector3 creationPosition)
        {
            Debug.Log("Creation request received from client " + clientId + " position " + creationPosition);
            if (objectType.Equals(ObjectEnum.Ball))
            {
                Ball newBall = _objectFactory.CreateBall(creationPosition, clientId);
                _creationRequests.Enqueue(newBall); 
                _eventManager.BroadcastEventAction(new CreationAction(newBall.Position, newBall.ObjectId, ObjectEnum.Ball));
                _eventManager.SendEventAction(new AssignPlayerAction(newBall.ObjectId), clientId);
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
}