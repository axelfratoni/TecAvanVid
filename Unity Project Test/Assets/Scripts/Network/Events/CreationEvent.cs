using System;
using Libs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Persistence;

namespace Network.Events
{
    
    public class CreationEvent : IEvent
    {
        private Vector3 position;

        public CreationEvent(Vector3 pos, int seq_id)
        {
            eventEnum = EventEnum.EventCreation;
            buffer = new BitBuffer(9);
            position = pos;
            this.seq_id = seq_id;
        }

        public override byte[] GetByteArray()
        {
            buffer.writeInt((int)eventEnum,0,(int)EventEnum.total-1);
            buffer.writeFloat(position.x,0f,5f,0.1f);
            buffer.writeFloat(position.y,0f,5f,0.1f);
            buffer.writeFloat(position.z,0f,5f,0.1f);
            buffer.writeInt(this.seq_id,0,Int32.MaxValue);
            return buffer.getBuffer();
        }

    }    
    
}