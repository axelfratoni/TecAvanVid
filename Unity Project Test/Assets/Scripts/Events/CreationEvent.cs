using System;
using Libs;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Persistence;

namespace Network.Events
{
    
    public class CreationEvent : IEvent
    {
        private static int BitsRequired;
        private GameObject _gameObject;

        //This will run only once
        static CreationEvent(){
            
            BitsRequired = seqBitsRequired + idBitsRequired;
        }

        public override int GetBitsRequired()
        {
            return BitsRequired;
        }

        public CreationEvent(int seq_id, int id)
        {
            eventEnum = EventEnum.EventCreation;
            buffer = new BitBuffer(BitsRequired);
            this.seq_id = seq_id;
            this.id = id;
        }

        public CreationEvent(BitBuffer bitBuffer)
        {
            seq_id = bitBuffer.readInt(0, Int32.MaxValue);
            id = bitBuffer.readInt(0, Int32.MaxValue);
        }

        public override byte[] GetByteArray()
        {
            buffer.writeInt((int)eventEnum,0,EventManager.GetTotalEventEnum());
            buffer.writeInt(seq_id,0,Int32.MaxValue);
            buffer.writeInt(id,0,Int32.MaxValue);
            return buffer.getBuffer();
        }

        public override void Process(GameObject gameObject)
        {
            //TODO GET A RANDOM MOVING BALL
            _gameObject = new GameObject("er");
            return;
        }

        public GameObject GameObject
        {
            get { return _gameObject; }
        }
        
        public override object Clone()
        {
            return new CreationEvent(seq_id, id);
        }
    }    
    
}