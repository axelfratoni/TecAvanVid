using System;
using System.Net;
using UnityEngine;

namespace Events
{
    public class Connection
    {
        public static int MAX_SEQ_ID = 500;
        
        private readonly int _id;
        private readonly UDPChannel _channel;
        private readonly IPEndPoint _sendingEndPoint;
        private int _seqId;

        public Connection(int id, UDPChannel channel, IPEndPoint sendingEndPoint)
        {
            _id = id;
            _channel = channel;
            _sendingEndPoint = sendingEndPoint;
            _seqId = 0;
        }

        public void Send(Event iEvent)
        {
            _channel.SendMessageTo(iEvent.Serialize(), _sendingEndPoint);
        }

        public int Id
        {
            get { return _id; }
        }

        public IPEndPoint GetSendingEndpoint()
        {
            return _sendingEndPoint;
        }

        public bool IsThisEndpoint(IPEndPoint endPoint)
        {
            return _sendingEndPoint.Equals(endPoint);
        }

        public EventBuilder AddSeqId(EventBuilder eventBuilder)
        {
            eventBuilder.SetSeqId(_seqId);
            _seqId = _seqId < MAX_SEQ_ID ? _seqId + 1 : 0;
            return eventBuilder;
        }

        public void Disable()
        {
            _channel.Disable();
        }

        ~Connection()
        {
            Disable();
        }
    }

    public class ConnectionFactory
    {
        private int _connectionId = 1;
        private readonly UDPChannel _channel;

        public ConnectionFactory(UDPChannel channel)
        {
            _channel = channel;
        }

        public Connection GetNewConnection(IPEndPoint remoteEndpoint)
        {
            Connection connection = new Connection(_connectionId++, _channel, remoteEndpoint);
            return connection;
        } 
    }
}