using System;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class SnapshotAction : EventAction
    {
        public const float MaxCycleTime = 3600;

        public int ObjectId { get; private set; }
        public Vector3 ObjectPosition { get; private set; }
        public Quaternion ObjectRotation { get; private set; }
        public double TimeStamp { get; private set; }
        public double LastClientInputTime { get; private set; }

        public SnapshotAction(int objectId, Vector3 objectPosition, Quaternion objectRotation, double timeStamp, double lastClientInputTime)
        {
            ObjectId = objectId;
            ObjectPosition = objectPosition;
            ObjectRotation = objectRotation;
            TimeStamp = timeStamp;
            LastClientInputTime = lastClientInputTime;
        }

        public SnapshotAction(BitBuffer payload)
        {
            TimeStamp = payload.readFloat(0.0f, MaxCycleTime, 0.01f);
            LastClientInputTime = payload.readFloat(0.0f, MaxCycleTime, 0.01f);
            ObjectId = payload.readInt(0, Int32.MaxValue);
            
            float x = payload.readFloat(-31.0f, 31.0f, 0.1f);
            float y = payload.readFloat(0.0f, 10.0f, 0.1f);
            float z = payload.readFloat(-31.0f, 31.0f, 0.1f);
            
            ObjectPosition = new Vector3(x, y, z);

            x = payload.readFloat(-1, 1, 0.1f);
            y = payload.readFloat(-1, 1, 0.1f);
            z = payload.readFloat(-1, 1, 0.1f);
            float w = payload.readFloat(-1, 1, 0.1f);
            
            ObjectRotation = new Quaternion(x, y, z, w); 
        }

        public override void Serialize(BitBuffer bitBuffer)
        {
            bitBuffer.writeFloat((float)TimeStamp, 0.0f, MaxCycleTime, 0.01f);
            bitBuffer.writeFloat((float)LastClientInputTime, 0.0f, MaxCycleTime, 0.01f);
            bitBuffer.writeInt(ObjectId, 0, Int32.MaxValue);
                
            bitBuffer.writeFloat(ObjectPosition.x, -31.0f, 31.0f, 0.1f);
            bitBuffer.writeFloat(ObjectPosition.y, 0.0f, 10.0f, 0.1f);
            bitBuffer.writeFloat(ObjectPosition.z, -31.0f, 31.0f, 0.1f);
            
            bitBuffer.writeFloat(ObjectRotation.x, -1, 1, 0.1f);
            bitBuffer.writeFloat(ObjectRotation.y, -1, 1, 0.1f);
            bitBuffer.writeFloat(ObjectRotation.z, -1, 1, 0.1f);
            bitBuffer.writeFloat(ObjectRotation.w, -1, 1, 0.1f);
        }
        
        public void Extract(Action<int, Vector3, Quaternion, double> executor, int clientId)
        {
            executor(ObjectId, ObjectPosition, ObjectRotation, TimeStamp);
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