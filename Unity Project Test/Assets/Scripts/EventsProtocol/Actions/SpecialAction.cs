using System;
using Libs;

namespace Events.Actions
{
    public class SpecialAction : EventAction
    {
        private readonly SpecialActionEnum _action;
        private readonly int _objectId;
        
        public SpecialAction(SpecialActionEnum action, int objetcId)
        {
            _action = action;
            _objectId = objetcId;
        }

        public SpecialAction(BitBuffer payload)
        {
            _action = (SpecialActionEnum) payload.readInt(0, Enum.GetValues(typeof(SpecialActionEnum)).Length);
            _objectId = payload.readInt(0, Int32.MaxValue);
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeInt((int) _action, 0, Enum.GetValues(typeof(SpecialActionEnum)).Length);
            buffer.writeInt(_objectId, 0, Int32.MaxValue);
        }
        
        public void Extract(Action<SpecialActionEnum, int> executor, int clientId)
        {
            executor(_action, _objectId);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.NoTimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Special;
        }
    }
    
}

public enum SpecialActionEnum
{
    FiringStart = 0,
    FiringStop = 1,
    Destroy = 2,
} 
