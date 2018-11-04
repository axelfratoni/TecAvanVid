using System;
using Libs;
using Network;
using UnityEngine;

namespace Events.Actions
{
    public class CreationRequestAction : EventAction
    {
        private readonly Vector3 _creationPosition;
        private readonly ObjectEnum _objectType;

        public CreationRequestAction(Vector3 creationPosition, ObjectEnum objectType)
        {
            _creationPosition = creationPosition;
            _objectType = objectType;
        }

        public CreationRequestAction(BitBuffer buffer)
        {
            
            _objectType = (ObjectEnum) buffer.readInt(0, Enum.GetValues(typeof(ObjectEnum)).Length);
            
            float x = buffer.readFloat(-31.0f, 31.0f, 0.1f);
            float y = buffer.readFloat(0.0f, 3.0f, 0.1f);
            float z = buffer.readFloat(-31.0f, 31.0f, 0.1f);
            
            _creationPosition = new Vector3(x, y, z);
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeInt((int) _objectType, 0, Enum.GetValues(typeof(ObjectEnum)).Length);
            
            buffer.writeFloat(_creationPosition.x, -31.0f, 31.0f, 0.1f);
            buffer.writeFloat(_creationPosition.y, 0.0f, 3.0f, 0.1f);
            buffer.writeFloat(_creationPosition.z, -31.0f, 31.0f, 0.1f);
        }

        public void Extract(Action<int, Vector3, ObjectEnum> executor, int clientId)
        {
            executor(clientId, _creationPosition, _objectType);
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
    
    public enum ObjectEnum
    {
        Ball = 0,
        Projectile = 1,
        Player = 2
    }
}