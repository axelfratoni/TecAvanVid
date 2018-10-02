using System;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class AssignPlayerAction : EventAction
    {
        private int _objectId;
        
        public AssignPlayerAction(int objectId)
        {
            _objectId = objectId;
        }
        
        public AssignPlayerAction(BitBuffer payload)
        {
            _objectId = payload.readInt(0, Int32.MaxValue);
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeInt(_objectId, 0, Int32.MaxValue);
        }

        public override void Execute(ActionDispatcher actionDispatcher, int clientId)
        {
            actionDispatcher.AssignPlayerAction(_objectId);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.TimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.AssignPlayer;
        }
    }
}