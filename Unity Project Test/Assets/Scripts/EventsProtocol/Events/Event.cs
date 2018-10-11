using System;
using System.Collections.Generic;
using Events.Actions;
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
        private readonly EventAction _payload;

        public Event(int seqId, int clientId, bool ack, EventEnum eventEnum, EventTimeoutTypeEnum timeoutTypeEnum, EventAction payload)
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
            buffer.writeInt(_seqId, 0, Connection.MAX_SEQ_ID);
            buffer.writeBit(_ack);
            buffer.writeInt((int)_eventEnum, 0, Enum.GetValues(typeof(EventEnum)).Length);
            buffer.writeInt((int)_timeoutTypeEnum, 0, Enum.GetValues(typeof(EventTimeoutTypeEnum)).Length);
            
            _payload.Serialize(buffer);

            buffer.flush();

            return buffer.getBuffer();
        }

        public static EventBuilder Deserialize(byte[] message)
        {
            BitBuffer buffer = new BitBuffer(message);
            
            int seqId = buffer.readInt(0, Connection.MAX_SEQ_ID);
            bool ack = buffer.readBit();
            EventEnum eventType= (EventEnum) buffer.readInt(0, Enum.GetValues(typeof(EventEnum)).Length);
            EventTimeoutTypeEnum eventTimeoutType =
                (EventTimeoutTypeEnum) buffer.readInt(0, Enum.GetValues(typeof(EventTimeoutTypeEnum)).Length);
            EventAction payload = EventAction.ExtractAction(buffer, eventType);
            
            EventBuilder eventBuilder = new EventBuilder().SetSeqId(seqId)
                                                          .SetAck(ack)
                                                          .SetEventType(eventType)
                                                          .SetTimeoutType(eventTimeoutType)
                                                          .SetPayload(payload);
            return eventBuilder;
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

        public EventAction GetPayload()
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
        private EventAction _payload;
        
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

        public EventBuilder SetPayload(EventAction payload)
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