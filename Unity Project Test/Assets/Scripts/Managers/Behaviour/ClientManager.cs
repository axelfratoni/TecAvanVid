using Events.Actions;

namespace Events
{
    public class ClientManager : BehaviourManager
    {
        private readonly EventManager _eventManager;

        public ClientManager(EventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public void Update()
        {
            InputMapper.ExtractInput().ForEach(input => _eventManager.SendEventAction(new MovementAction(0, input), 1));
        }
    }
}