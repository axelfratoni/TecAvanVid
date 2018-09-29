using Events.Actions;

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
            eventManager.SendEventAction(new CreationRequestAction(), serverId);
        }

        public void Update()
        {
            InputMapper.ExtractInput().ForEach(input => _eventManager.SendEventAction(new MovementAction(0, input), _serverId));
        }
    }
}