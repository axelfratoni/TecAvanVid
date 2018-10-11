using Libs;

namespace Events.Actions
{
    public class ConnectionAction : EventAction
    {
        public ConnectionAction(){}
        
        public ConnectionAction(BitBuffer buffer){}

        public override void Serialize(BitBuffer buffer)
        {
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.TimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Connection;
        }
    }
}