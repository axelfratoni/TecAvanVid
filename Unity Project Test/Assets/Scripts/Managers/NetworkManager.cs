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
            
            Event ievent = Event.Deserialize(receivedBytes).SetClientId(connectionId)
                                                           .Build();    
            
            //Debug.Log("Receiving from: " + remoteEndpoint + "\nclient: "+ ievent.ClientId + " - seqId: " + ievent.SeqId + " - ack: " + ievent.Ack + " - timeout: " + ievent.GetTimeoutType() + " - type: " + ievent.GetEventEnum() );
            if (ievent.GetEventEnum().Equals(EventEnum.Connection))
            {
                ievent = HandleConnectionEvent(ievent, remoteEndpoint);
            }
            _eventManager.ReceiveEvent(ievent);
        }

        public void SendEvent(Event ievent)
        {
            Connection connection = _connectionList.Find(con => con.Id == ievent.ClientId);
            if (connection != null)
            {
                //Debug.Log("Sending to: " + connection.GetSendingEndpoint() + "\nclient: "+ ievent.ClientId + " - seqId: " + ievent.SeqId + " - ack: " + ievent.Ack + " - timeout: " + ievent.GetTimeoutType() + " - type: " + ievent.GetEventEnum());
                connection.Send(ievent);
            }
            else
            {
                throw new Exception("No such connection " + ievent.ClientId);
            }
        }

        private Event HandleConnectionEvent(Event ievent, IPEndPoint remoteEndpoint)
        {
            if (ievent.ClientId == -1)
            {
                int newId = AddConnection(remoteEndpoint);
                Event verifiedEvent = new EventBuilder(ievent).SetClientId(newId)
                                                              .Build();
                return verifiedEvent;
            }
            if (ievent.ClientId != -1 && ievent.Ack)
            {
                _eventManager.ConfirmConnection(ievent.ClientId);
            }
            return ievent;
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

        public List<int> GetClientIdList()
        {
            List<int> clientIdList = new List<int>();
            _connectionList.ForEach(connection => clientIdList.Add(connection.Id));
            return clientIdList;
        }

        public void Disable()
        {
            _connectionList.ForEach(con => con.Disable());
            _receiver.Disable();
        }
    }
}