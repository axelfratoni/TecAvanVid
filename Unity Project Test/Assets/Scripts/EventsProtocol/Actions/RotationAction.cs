using System;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class RotationAction : EventAction
    {
        private readonly Quaternion _rotation;

        public RotationAction(Quaternion rotation)
        {
            _rotation = rotation;
        }

        public RotationAction(BitBuffer payload)
        {
            float x = payload.readFloat(-1, 1, 0.1f);
            float y = payload.readFloat(-1, 1, 0.1f);
            float z = payload.readFloat(-1, 1, 0.1f);
            float w = payload.readFloat(-1, 1, 0.1f);
            
            _rotation = new Quaternion(x, y, z, w); 
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeFloat(_rotation.x, -1, 1, 0.1f);
            buffer.writeFloat(_rotation.y, -1, 1, 0.1f);
            buffer.writeFloat(_rotation.z, -1, 1, 0.1f);
            buffer.writeFloat(_rotation.w, -1, 1, 0.1f);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.NoTimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Rotation;
        }

        public void Extract(Action<int, Quaternion> processRotation, int iEventClientId)
        {
            processRotation(iEventClientId, _rotation);
        }
    }
}