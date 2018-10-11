using Libs;

namespace Events.Actions
{
    public class DamageAction : EventAction
    {
        public DamageAction()
        {
            
        }

        public DamageAction(BitBuffer payload)
        {
            
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.NoTimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.ReceiveDamage;
        }
    }
}