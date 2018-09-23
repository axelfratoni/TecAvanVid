using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Events.Actions;
using Libs;
using UnityEngine;

namespace Events
{
    public class NetworkManager
    {
        private readonly List<Connection> _connectionList;
        private readonly UDPChannel _receiver;
        private readonly ConnectionFactory _connectionFactory;
        private readonly EventManager _eventManager;

        public NetworkManager(int localPort, EventManager eventManager)
        {
            _connectionList = new List<Connection>();
            _receiver = new UDPChannel(localPort, ReceiveEvent);
            _connectionFactory = new ConnectionFactory(_receiver);
            _eventManager = eventManager;
        }
        
        public int AddConnection(IPEndPoint remoteEndpoint)
        {
            Connection connection = _connectionFactory.GetNewConnection(remoteEndpoint);
            _connectionList.Add(connection);
            return connection.Id;
        }
        
        private void ReceiveEvent(byte[] receivedBytes, IPEndPoint remoteEndpoint)
        {
            Connection connection = _connectionList.Find(con => con.IsThisEndpoint(remoteEndpoint));
            int connectionId = connection == null ? -1 : connection.Id;
            
            Event ievent = new EventBuilder().DeserializeMessage(receivedBytes)
                                             .SetClientId(connectionId)
                                             .Build();
            
            Debug.Log("Receiving client "+ ievent.ClientId + " type " + ievent.GetEventEnum() + " ack " + ievent.Ack);
            if (ievent.ClientId == -1 && ievent.GetEventEnum().Equals(EventEnum.Connection))
            {
                int newId = AddConnection(remoteEndpoint);
                ievent = new EventBuilder(ievent).SetClientId(newId)
                                                 .Build();
            }
            _eventManager.ReceiveEvent(ievent);
        }

        public void SendEvent(Event ievent)
        {
            Connection connection = _connectionList.Find(con => con.Id == ievent.ClientId);
            if (connection != null)
            {
                connection.Send(ievent);
            }
            else
            {
                throw new Exception("No such connection " + ievent.ClientId);
            }
        }

        public EventBuilder AddNetworkInfo(EventBuilder eventBuilder, int connectionId)
        {
            Connection connection = _connectionList.Find(con => con.Id == connectionId);
            if (connection != null)
            {
                connection.AddSeqId(eventBuilder);
            }
            else
            {
                throw new Exception("No such connection " + connectionId);
            }

            return eventBuilder;
        }

        public void Disable()
        {
            _connectionList.ForEach(con => con.Disable());
            _receiver.Disable();
        }
    }
}