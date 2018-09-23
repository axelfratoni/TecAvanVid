using System.Net;
using UnityEngine;

namespace Events
{
    public class Connection
    {
        private readonly int _id;
        private readonly UDPChannel _channel;
        private readonly IPEndPoint _sendingEndPoint;
        private int seqId;

        public Connection(int id, UDPChannel channel, IPEndPoint sendingEndPoint)
        {
            _id = id;
            _channel = channel;
            _sendingEndPoint = sendingEndPoint;
            seqId = 0;
        }

        public void Send(Event iEvent)
        {
            Debug.Log("Sending client "+ iEvent.ClientId + " size "+ iEvent.GetPayload().Length +" ack " + iEvent.Ack);
            if (iEvent.Ack)
            {
                Debug.Log(_sendingEndPoint);
            }
            _channel.SendTo(iEvent.Serialize(), _sendingEndPoint);
        }

        public int Id
        {
            get { return _id; }
        }

        public bool IsThisEndpoint(IPEndPoint endPoint)
        {
            return _sendingEndPoint.Equals(endPoint);
        }

        public EventBuilder AddSeqId(EventBuilder eventBuilder)
        {
            return eventBuilder.SetSeqId(seqId++);
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