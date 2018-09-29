using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class CreationAction : EventAction
    {
        private readonly Vector3 _creationPosition;

        public CreationAction(Vector3 creationPosition)
        {
            _creationPosition = creationPosition;
        }
        
        public CreationAction(BitBuffer payload)
        {
            float x = payload.readFloat(-31.0f, 31.0f, 0.1f);
            float y = payload.readFloat(0.0f, 3.0f, 0.1f);
            float z = payload.readFloat(-31.0f, 31.0f, 0.1f);
            
            _creationPosition = new Vector3(x, y, z);
        }

        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeFloat(_creationPosition.x, -31.0f, 31.0f, 0.1f);
            buffer.writeFloat(_creationPosition.y, 0.0f, 3.0f, 0.1f);
            buffer.writeFloat(_creationPosition.z, -31.0f, 31.0f, 0.1f);
        }

        public override void Execute(WorldManager worldManager, int clientId)
        {
            worldManager.ProcessObjectCreation(_creationPosition, clientId);
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