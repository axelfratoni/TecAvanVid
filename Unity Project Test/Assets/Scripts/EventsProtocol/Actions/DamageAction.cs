using System;
using Libs;

namespace Events.Actions
{
    public class DamageAction : EventAction
    {
        private readonly int _currentHealth;
        private readonly int _objectId;
        
        public DamageAction(int currentHealth, int objectId)
        {
            _currentHealth = currentHealth;
            _objectId = objectId;
        }

        public DamageAction(BitBuffer payload)
        {
            _currentHealth = payload.readInt(0, 100);
            _objectId = payload.readInt(0, Int32.MaxValue);
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeInt(_currentHealth, 0, 100);
            buffer.writeInt(_objectId, 0, Int32.MaxValue);
        }

        public void Extract(Action<int, int> extractor, int clientId)
        {
            extractor(_currentHealth, _objectId);
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