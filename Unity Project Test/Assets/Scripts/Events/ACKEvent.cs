using System;
using Libs;
using UnityEngine;

namespace Network.Events
{
    public class ACKEvent : IEvent
    {
        private static int TimeoutTypeBitsRequired;
        private static int SeqBitsRequired;
        private static readonly int BitsRequired;
        
        private readonly EventTimeoutTypeEnum _timeoutTypeEnum;
        
        //This will run only once
        static ACKEvent()
        {
            TimeoutTypeBitsRequired = BitBuffer.GetBitsRequired(EventManager.GetTotalEventTimeoutTypeEnum());
            SeqBitsRequired = BitBuffer.GetBitsRequired(Int32.MaxValue);
            BitsRequired = TimeoutTypeBitsRequired + SeqBitsRequired;
        }

        public override int GetBitsRequired()
        {
            return BitsRequired;
        }

        public ACKEvent(EventTimeoutTypeEnum timeoutType, int seq_id)
        {
            eventEnum = EventEnum.ACK;
            buffer = new BitBuffer(BitsRequired);
            _timeoutTypeEnum = timeoutType;
            this.seq_id = seq_id;
        }

        public ACKEvent(BitBuffer bitBuffer)
        {
            int timeoutType = bitBuffer.readInt(1,EventManager.GetTotalEventTimeoutTypeEnum());
            _timeoutTypeEnum = (EventTimeoutTypeEnum)timeoutType;
            seq_id = bitBuffer.readInt(0, Int32.MaxValue);
        }

        public override void Process(GameObject gameObject)
        {
            // Do Nothing
        }

        public override byte[] GetByteArray()
        {
            buffer.writeInt((int) eventEnum, 1, EventManager.GetTotalEventEnum());
            buffer.writeInt((int)_timeoutTypeEnum, 1, EventManager.GetTotalEventTimeoutTypeEnum());
            buffer.writeInt(seq_id, 0, Int32.MaxValue);
            return buffer.getBuffer();
        }

        public EventTimeoutTypeEnum TimeoutTypeEnum
        {
            get { return _timeoutTypeEnum; }
        }
        
        public override object Clone()
        {
            return new ACKEvent(_timeoutTypeEnum,seq_id);
        }
    }
}