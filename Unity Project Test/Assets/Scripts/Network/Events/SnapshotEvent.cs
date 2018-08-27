using System;
using Libs;
using UnityEngine;

namespace Network.Events
{
    public class SnapshotEvent : IEvent
    {
        private Vector3 position;
        private static int xBitsRequired;
        private static int yBitsRequired;
        private static int zBitsRequired;
        private static int seqBitsRequired;
        private static int bitsRequired;

        //This will run only once
        static SnapshotEvent()
        {

            xBitsRequired = BitBuffer.GetBitsRequiredForFloat(-31.0f, 31.0f, 0.1f);
            yBitsRequired = BitBuffer.GetBitsRequiredForFloat(0.0f, 3.0f, 0.1f);
            zBitsRequired = BitBuffer.GetBitsRequiredForFloat(-31.0f, 31.0f, 0.1f);
            seqBitsRequired = BitBuffer.GetBitsRequired(Int32.MaxValue);
            bitsRequired = xBitsRequired + yBitsRequired + zBitsRequired + seqBitsRequired;
        }

        public static int GetBitsRequired()
        {
            return bitsRequired;
        }

        public SnapshotEvent(Vector3 pos, int seq_id)
        {
            eventEnum = EventEnum.Snapshot;
            buffer = new BitBuffer(bitsRequired);
            position = pos;
            this.seq_id = seq_id;
        }

        public SnapshotEvent(BitBuffer bitBuffer)
        {
            float x = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
            float y = bitBuffer.readFloat(0.0f, 3.0f, 0.1f);
            float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
            position = new Vector3(x, y, z);
            seq_id = bitBuffer.readInt(0, Int32.MaxValue);
        }

        public override void Process(GameObject gameObject)
        {
            gameObject.transform.position = position;
        }

        public override byte[] GetByteArray()
        {

            buffer.writeInt((int) eventEnum, 1, EventManager.GetTotalEventEnum());
            buffer.writeFloat(position.x, 0f, 5f, 0.1f);
            buffer.writeFloat(position.y, 0f, 5f, 0.1f);
            buffer.writeFloat(position.z, 0f, 5f, 0.1f);
            buffer.writeInt(seq_id, 0, Int32.MaxValue);
            return buffer.getBuffer();
        }
    }
}