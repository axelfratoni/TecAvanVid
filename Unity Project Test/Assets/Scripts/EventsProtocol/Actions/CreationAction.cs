using System;
using Libs;
using Network;
using UnityEngine;

namespace Events.Actions
{
    public class CreationAction : EventAction
    {
        private readonly Vector3 _creationPosition;
        private readonly int _objectId;
        private readonly ObjectEnum _objectType;

        public CreationAction(Vector3 creationPosition, int objectId, ObjectEnum objectType)
        {
            _creationPosition = creationPosition;
            _objectId = objectId;
            _objectType = objectType;
        }
        
        public CreationAction(BitBuffer payload)
        {
            _objectType = (ObjectEnum) payload.readInt(0, Enum.GetValues(typeof(ObjectEnum)).Length);
            
            float x = payload.readFloat(-31.0f, 31.0f, 0.1f);
            float y = payload.readFloat(0.0f, 3.0f, 0.1f);
            float z = payload.readFloat(-31.0f, 31.0f, 0.1f);
            
            _creationPosition = new Vector3(x, y, z);

            _objectId = payload.readInt(0, Int32.MaxValue);
        }

        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeInt((int) _objectType, 0, Enum.GetValues(typeof(ObjectEnum)).Length);
            
            buffer.writeFloat(_creationPosition.x, -31.0f, 31.0f, 0.1f);
            buffer.writeFloat(_creationPosition.y, 0.0f, 3.0f, 0.1f);
            buffer.writeFloat(_creationPosition.z, -31.0f, 31.0f, 0.1f);
            
            buffer.writeInt(_objectId, 0, Int32.MaxValue);
        }

        public void Extract(Action<int, Vector3, ObjectEnum, int> executor, int clientId)
        {
            executor(_objectId, _creationPosition, _objectType, clientId);
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