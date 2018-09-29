using System.Collections.Generic;
using Events.Actions;
using Network;
using UnityEngine;

namespace Events
{
    public class ClientManager : BehaviourManager
    {
        private readonly EventManager _eventManager;
        private readonly int _serverId;

        public ClientManager(EventManager eventManager, int serverId)
        {
            _eventManager = eventManager;
            _serverId = serverId;
            eventManager.SendEventAction(new CreationRequestAction(new Vector3(0, 1, 2), ObjectEnum.Ball), serverId);
        }

        public void Update()
        {
            List<InputEnum> inputList = InputMapper.ExtractInput();
            if (inputList.Count > 0)
            {
                inputList.ForEach(input =>
                    _eventManager.SendEventAction(new MovementAction(0, input), _serverId)); // TODO que envie una lista de inputs
            }
        }
    }
}