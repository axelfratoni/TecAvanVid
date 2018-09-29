using Libs;

namespace Events.Actions
{
    public class CreationRequestAction : EventAction
    {
        public CreationRequestAction(){}
        
        public CreationRequestAction(BitBuffer buffer){}
        
        public override void Serialize(BitBuffer buffer)
        {
        }

        public override void Execute(WorldManager worldManager, int clientId)
        {
            worldManager.ProcessCreationRequest(clientId);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.TimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.CreationRequest;
        }
    }
}