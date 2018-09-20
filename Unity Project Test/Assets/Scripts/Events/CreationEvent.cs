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
        private BallEric _ball;
        private int _playerId;

        //This will run only once
        static CreationEvent(){
            
            BitsRequired = seqBitsRequired + idBitsRequired;
        }

        public override int GetBitsRequired()
        {
            return BitsRequired;
        }

        /*Server use*/
        public CreationEvent(int seq_id, int id, int clientId)
        {
            eventEnum = EventEnum.EventCreation;
            buffer = new BitBuffer(BitsRequired);
            this.seq_id = seq_id;
            this.id = id;
            _playerId = clientId;
        }
        
        /*Client use*/
        public CreationEvent(int seq_id, int clientId)
        {
            eventEnum = EventEnum.EventCreation;
            buffer = new BitBuffer(BitsRequired);
            this.seq_id = seq_id;
            _playerId = clientId;
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

        public override void Process(BallEric ball)
        {
            //TODO CALL MANAGER AND GET A RANDOM MOVING BALL
            //_ball = new BallEric();
            return;
        }

        public BallEric Ball
        {
            get { return _ball; }
        }
        
        public override object Clone()
        {
            return new CreationEvent(seq_id, id);
        }
    }    
    
}