using System;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class SnapshotAction : EventAction
    {
        private readonly int _objectId;
        private readonly Vector3 _objectPosition;
        private readonly double _timeStamp;

        public SnapshotAction(int objectId, Vector3 objectPosition, double timeStamp)
        {
            _objectId = objectId;
            _objectPosition = objectPosition;
            _timeStamp = timeStamp;
        }

        public SnapshotAction(BitBuffer payload)
        {
            _timeStamp = payload.readFloat(0.0f, 3600.0f, 0.01f);
            _objectId = payload.readInt(0, Int32.MaxValue);
            
            float x = payload.readFloat(-31.0f, 31.0f, 0.1f);
            float y = payload.readFloat(0.0f, 3.0f, 0.1f);
            float z = payload.readFloat(-31.0f, 31.0f, 0.1f);
            
            _objectPosition = new Vector3(x, y, z);
        }

        public override void Serialize(BitBuffer bitBuffer)
        {
            bitBuffer.writeFloat((float)_timeStamp, 0.0f, 3600.0f, 0.01f);
            bitBuffer.writeInt(_objectId, 0, Int32.MaxValue);
                
            bitBuffer.writeFloat(_objectPosition.x, -31.0f, 31.0f, 0.1f);
            bitBuffer.writeFloat(_objectPosition.y, 0.0f, 3.0f, 0.1f);
            bitBuffer.writeFloat(_objectPosition.z, -31.0f, 31.0f, 0.1f);
        }
        
        public override void Execute(GameManager gameManager)
        {
            gameManager.ProcessSnapshot(_objectId, _timeStamp, _objectPosition);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.Unreliable;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Snapshot;
        }
    }
}