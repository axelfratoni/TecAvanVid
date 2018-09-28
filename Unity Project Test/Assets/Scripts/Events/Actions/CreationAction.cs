using Libs;

namespace Events.Actions
{
    public class CreationAction : EventAction
    {
        public CreationAction(BitBuffer payload)
        {
            
        }

        public override void Serialize(BitBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(WorldManager worldManager)
        {
            throw new System.NotImplementedException();
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.TimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Creation;
        }
    }
}