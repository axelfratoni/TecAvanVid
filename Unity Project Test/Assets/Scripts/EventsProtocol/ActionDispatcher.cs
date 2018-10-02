using System.Collections.Generic;
using Events.Actions;
using Network;
using UnityEngine;

namespace Events
{
    public class ActionDispatcher
    {
        private readonly ClientManager _clientManager;
        private readonly ServerManager _serverManager;
        
        public ActionDispatcher(ClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public ActionDispatcher(ServerManager serverManager)
        {
            _serverManager = serverManager;
        }
        
        // Client Actions
        
        public void InitializeGame(int serverId)
        {
            _clientManager.InitializeGame(serverId);
        }

        public void ProcessSnapshot(int objectId, double timeStamp, Vector3 objectPosition)
        {
            _clientManager.ProcessSnapshot(objectId, timeStamp, objectPosition);
        }

        public void ProcessObjectCreation(Vector3 creationPosition, int clientId, int objectId, ObjectEnum objectType)
        {
            _clientManager.ProcessObjectCreation(creationPosition, clientId, objectId, objectType);
        }
        
        public void AssignPlayerAction(int objectId)
        {
            _clientManager.ProcessAssignPlayerAction(objectId);
        }
        
        // Server Actions
        
        public void ProcessInput(double time, List<InputEnum> inputList, int clientId)
        {
            _serverManager.ProcessInput(time, inputList, clientId);
        }

        public void ProcessCreationRequest(int clientId, ObjectEnum objectType, Vector3 creationPosition)
        {
            _serverManager.ProcessCreationRequest(clientId, objectType, creationPosition);
        }

    }
}