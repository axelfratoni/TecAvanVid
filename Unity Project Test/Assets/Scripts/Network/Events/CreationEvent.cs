using System;
using Libs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Persistence;

namespace Network.Events
{
    
    public class CreationEvent : IEvent
    {
        private static int seqBitsRequired;
        private static int bitsRequired;
        private GameObject _gameObject;

        //This will run only once
        static CreationEvent(){
            
            seqBitsRequired = BitBuffer.GetBitsRequired(Int32.MaxValue);
            bitsRequired = seqBitsRequired;
        }

        public static int GetBitsRequired()
        {
            return bitsRequired;
        }

        public CreationEvent(int seq_id)
        {
            eventEnum = EventEnum.EventCreation;
            buffer = new BitBuffer(bitsRequired);
            this.seq_id = seq_id;
        }

        public CreationEvent(BitBuffer bitBuffer)
        {
            seq_id = bitBuffer.readInt(0, Int32.MaxValue);
        }

        public override byte[] GetByteArray()
        {
            buffer.writeInt((int)eventEnum,0,EventManager.GetTotalEventEnum());
            buffer.writeInt(seq_id,0,Int32.MaxValue);
            return buffer.getBuffer();
        }

        public override void Process(GameObject gameObject)
        {
            // TODO New Object
            return;
        }

        public GameObject GameObject
        {
            get { return _gameObject; }
        }
    }    
    
}