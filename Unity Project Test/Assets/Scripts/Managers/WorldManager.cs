using System;
using System.Collections.Generic;
using Events.Actions;
using Network;
using UnityEngine;

namespace Events
{
    public class WorldManager : MonoBehaviour
    {
        public GameObject ballPrefab;

        private ObjectFactory _objectFactory;
        private EventManager _eventManager;
        private readonly List<BallController> _balls = new List<BallController>();
        private readonly Queue<Ball> _creationRequests = new Queue<Ball>();

        private void Start()
        {
            _objectFactory = new ObjectFactory();
        }

        public void SetEventManager(EventManager eventManager)
        {
            _eventManager = eventManager;
        }
        
        private void Update()
        {
            while (_creationRequests.Count > 0)
            {
                Ball ball = _creationRequests.Dequeue();                
                BallController ballController = Instantiate(ballPrefab).GetComponent<BallController>();
                ballController.SetBall(ball);
                _balls.Add(ballController);
            }
        }

        public List<BallController> GetBallList()
        {
            return _balls;
        }

        public void ProcessInput(double time, InputEnum input, int clientId) // TODO que reciba una lista de inputs
        {
            Debug.Log("Received input");
            BallController ballController = _balls.Find(ball => ball.GetBall().ClientId.Equals(clientId));
            if(ballController != null) ballController.ApplyInput(time, input);
        }

        public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
        {
            //Debug.Log("ReceiverSnapshot: " + objectPosition);
            BallController ballCont = _balls.Find(ball => ball.GetBall().ObjectId.Equals(objectId));
            //if(ballCont != null) ballCont.ApplySnapshot(timeStamp, objectPosition);
        }

        public void ProcessColorAction(int r, int g, int b)
        {
            Debug.Log("Received color: r " + r + " g " + g + " b " + b);
        }

        public void ProcessCreationRequest(int clientId, ObjectEnum objectType, Vector3 creationPosition)
        {
            Debug.Log("Creation request received from client " + clientId + " position " + creationPosition);
            if (objectType.Equals(ObjectEnum.Ball))
            {
                Ball newBall = _objectFactory.CreateBall(creationPosition, clientId);
                _creationRequests.Enqueue(newBall); 
                _eventManager.BroadcastEventAction(new CreationAction(newBall.Position, newBall.ObjectId, ObjectEnum.Ball));
            }
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