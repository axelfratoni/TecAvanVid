using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Events.Actions;
using Libs;
using UnityEngine;
using Random = System.Random;

namespace Events
{
    public class NetworkManager
    {
        private readonly List<Connection> _connectionList;
        private readonly List<LatencyEvent> _latencyEventList;
        private readonly UDPChannel _receiver;
        private readonly ConnectionFactory _connectionFactory;
        private readonly EventManager _eventManager;
        private readonly Random _random;

        public NetworkManager(int localPort, EventManager eventManager)
        {
            _connectionList = new List<Connection>();
            _latencyEventList = new List<LatencyEvent>();
            _receiver = new UDPChannel(localPort, ReceiveEvent);
            _connectionFactory = new ConnectionFactory(_receiver);
            _eventManager = eventManager;
            _random = new Random();
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
            
            var value = _random.NextDouble() * 100;
            if (value < Data.LOSS_PERCENTAGE)
            {
                return;
            }
            
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
            /* Add to latency*/
            double latency = Data.MIN_LATENCY + _random.NextDouble() * Data.MAX_LATENCY;
            _latencyEventList.Add(new LatencyEvent(ievent,latency));
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

        public void UpdateLatency(double deltaTime)
        {
            List<LatencyEvent> toRemove = new List<LatencyEvent>();
            foreach (var latencyEvent in _latencyEventList)
            {
                latencyEvent.Latency -= deltaTime;
                if (latencyEvent.Latency <= 0)
                {
                    Connection connection = _connectionList.Find(con => con.Id == latencyEvent.IEvent.ClientId);
                    if (connection != null)
                    {
                        //Debug.Log("Sending to: " + connection.GetSendingEndpoint() + "\nclient: "+ ievent.ClientId + " - seqId: " + ievent.SeqId + " - ack: " + ievent.Ack + " - timeout: " + ievent.GetTimeoutType() + " - type: " + ievent.GetEventEnum());
                        connection.Send(latencyEvent.IEvent);
                        toRemove.Add(latencyEvent);
                    }
                    else
                    {
                        throw new Exception("No such connection " + latencyEvent.IEvent.ClientId);
                    }

                }
            }
            _latencyEventList.RemoveAll(item => toRemove.Contains(item));
        }
        
        
    }
}