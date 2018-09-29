using System.Collections.Generic;
using Events.Actions;
using Network;
using UnityEngine;

namespace Events
{
    public class WorldManager : MonoBehaviour
    {
        public GameObject ballPrefab;

        private EventManager _eventManager;
        private readonly List<BallController> _balls = new List<BallController>();
        private readonly Queue<Ball> _creationRequests = new Queue<Ball>();

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

        public void ProcessInput(double time, InputEnum input, int clientId) // TODO que reciba una lista de inputs
        {
            Debug.Log("Received input");
            BallController ballController = _balls.Find(ball => ball.GetBall().ClientId.Equals(clientId));
            if(ballController != null) ballController.ApplyInput(time, input);
        }

        public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
        {
        }

        public void ProcessColorAction(int r, int g, int b)
        {
            Debug.Log("Received color: r " + r + " g " + g + " b " + b);
        }

        public void ProcessCreationRequest(int clientId)
        {
            Debug.Log("Creation request received from client " + clientId);
            _creationRequests.Enqueue(new Ball(clientId, 1, new Vector3(1,1,1))); // TODO: Esto esta a modo de prueba.
            _eventManager.BroadcastEventAction(new CreationAction(new Vector3(1,1,1)));
        }

        public void ProcessObjectCreation(Vector3 creationPosition, int clientId)
        {
            Debug.Log("Creation action received from server.");
            _creationRequests.Enqueue(new Ball(clientId, 1, new Vector3(1,1,1))); // TODO: Esto esta a modo de prueba.
        }
    }
}