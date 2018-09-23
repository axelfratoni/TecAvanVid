using System;
using Libs;
using UnityEngine;

namespace Events
{
    public class Event
    {
        private readonly int _clientId;
        
        // Message Header
        private readonly int _seqId;
        private readonly bool _ack;
        private readonly EventEnum _eventEnum;
        private readonly EventTimeoutTypeEnum _timeoutTypeEnum;
        
        // Message Content
        private readonly byte[] _payload;

        public Event(int seqId, int clientId, bool ack, EventEnum eventEnum, EventTimeoutTypeEnum timeoutTypeEnum, byte[] payload)
        {
            _clientId = clientId;
            
            _seqId = seqId;
            _ack = ack;
            _eventEnum = eventEnum;
            _timeoutTypeEnum = timeoutTypeEnum;
            _payload = payload;
        }

        public byte[] Serialize()
        {
            BitBuffer buffer = new BitBuffer(1024);
            buffer.writeInt(_seqId, 0, Int32.MaxValue);
            buffer.writeBit(_ack);
            buffer.writeInt((int)_eventEnum, 0, Enum.GetValues(typeof(EventEnum)).Length);
            buffer.writeInt((int)_timeoutTypeEnum, 0, Enum.GetValues(typeof(EventTimeoutTypeEnum)).Length);
            
            buffer.flush();
            byte[] header = buffer.getBuffer();
            
            byte[] message = new byte[header.Length + _payload.Length];
            header.CopyTo(message, 0);
            _payload.CopyTo(message, header.Length);

            return message;
        }

        public int SeqId
        {
            get { return _seqId; }
        }

        public int ClientId
        {
            get { return _clientId; }
        }

        public bool Ack
        {
            get { return _ack; }
        }

        public EventEnum GetEventEnum()
        {
            return _eventEnum;
        }

        public EventTimeoutTypeEnum GetTimeoutType()
        {
            return _timeoutTypeEnum;
        }

        public byte[] GetPayload()
        {
            return _payload;
        }
    }

    public class EventBuilder
    {
        private int _seqId;
        private int _clientId;
        private bool _ack;
        private EventEnum _eventEnum;
        private EventTimeoutTypeEnum _timeoutTypeEnum;
        private byte[] _payload;

        private static EventBitsRequired _eventBitsRequired = new EventBitsRequired();
        
        public EventBuilder(){}

        public EventBuilder(Event ievent)
        {
            _seqId = ievent.SeqId;
            _clientId = ievent.ClientId;
            _ack = ievent.Ack;
            _eventEnum = ievent.GetEventEnum();
            _timeoutTypeEnum = ievent.GetTimeoutType();
            _payload = ievent.GetPayload();
        }

        public EventBuilder DeserializeMessage(byte[] message)
        {
            BitBuffer buffer = new BitBuffer(message);
            
            // Read Header
            _seqId = buffer.readInt(0, Int32.MaxValue);
            _ack = buffer.readBit();
            _eventEnum = (EventEnum) buffer.readInt(0, Enum.GetValues(typeof(EventEnum)).Length);
            _timeoutTypeEnum = (EventTimeoutTypeEnum) buffer.readInt(0, Enum.GetValues(typeof(EventTimeoutTypeEnum)).Length);
            
            // Read payload
            //_payload = buffer.GetBuffer(_eventBitsRequired.GetBitsRequired(_eventEnum));
            //_payload = buffer.getBuffer(); //TODO: Hacer que ande esto.
            _payload = new byte[0];
            return this;
        }

        public EventBuilder SetSeqId(int seqId)
        {
            _seqId = seqId;
            return this;
        }
        
        public EventBuilder SetClientId(int clientId)
        {
            _clientId = clientId;
            return this;
        }

        public EventBuilder SetAck(bool ack)
        {
            _ack = ack;
            return this;
        }

        public EventBuilder SetEventType(EventEnum eventEnum)
        {
            _eventEnum = eventEnum;
            return this;
        }

        public EventBuilder SetTimeoutType(EventTimeoutTypeEnum eventTimeoutTypeEnum)
        {
            _timeoutTypeEnum = eventTimeoutTypeEnum;
            return this;
        }

        public EventBuilder SetPayload(byte[] payload)
        {
            _payload = payload;
            return this;
        }

        public Event Build()
        {
            return new Event(_seqId, _clientId, _ack, _eventEnum, _timeoutTypeEnum, _payload);
        }
    }
    
    
}