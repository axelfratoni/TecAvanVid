using System;
using System.Collections.Generic;
using System.Net;
using Events.Actions;
using UnityEngine;

namespace Events
{
    public class EventManager
    {
        private readonly NetworkManager _networkManager;
        private readonly Queue<Event> _unreadReceivedEvents;
        private readonly Dictionary<EventTimeoutTypeEnum, ReliableEventQueue> _sendingEventsQueues;
        private readonly Action<int> _initializationAction;
        
        public EventManager(int localPort, Action<int> initializationAction, int delay, float packetLoss)
        {
            _initializationAction = initializationAction;
            _unreadReceivedEvents = new Queue<Event>();
            _networkManager = new NetworkManager(localPort, this, delay, packetLoss);
            _sendingEventsQueues = new Dictionary<EventTimeoutTypeEnum, ReliableEventQueue>
            {
                {EventTimeoutTypeEnum.NoTimeOut, new ReliableEventQueue(5, SendEventsInQueue)},
                {EventTimeoutTypeEnum.TimeOut, new ReliableEventQueue(1000, SendEventsInQueue)}
            };
        }

        private void SendEventsInQueue(List<Event> eventQueue)
        {
            eventQueue.ForEach(ev =>
            {
                _networkManager.SendEventFakingLatencyAndPacketLoss(ev);
            });
        }

        public void SendEventAction(EventAction eventAction, int clientId)
        {
            EventBuilder eventBuilder = _networkManager.AddNetworkInfo(new EventBuilder(), clientId);
            Event ievent = eventBuilder.SetAck(false)
                                       .SetClientId(clientId)
                                       .SetTimeoutType(eventAction.GetTimeoutType())
                                       .SetEventType(eventAction.GetEventType())
                                       .SetPayload(eventAction)
                                       .Build();
            _networkManager.SendEventFakingLatencyAndPacketLoss(ievent);

            AddEventToReliableQueue(ievent);
        }

        public void BroadcastEventAction(EventAction eventAction)
        {
            _networkManager.GetClientIdList().ForEach(id => SendEventAction(eventAction, id));
        }

        private void AddEventToReliableQueue(Event ievent)
        {
            if (!ievent.GetTimeoutType().Equals(EventTimeoutTypeEnum.Unreliable))
            {
                ReliableEventQueue eventQueue;
                if (_sendingEventsQueues.TryGetValue(ievent.GetTimeoutType(), out eventQueue))
                {
                    eventQueue.AddEvent(ievent);  
                }
                else
                {
                    throw new Exception("No such timeout type " + ievent.GetTimeoutType());
                }
            }
        }

        public void ReceiveEvent(Event ievent)
        {
            ReliableEventQueue eventQueue;
            if (_sendingEventsQueues.TryGetValue(ievent.GetTimeoutType(), out eventQueue))
            {
                if (ievent.Ack)
                {
                    eventQueue.AckEvent(ievent);
                }
                else
                {
                    if (eventQueue.ShouldProcessEvent(ievent) && ievent.GetPayload() != null)
                    {
                        lock (_unreadReceivedEvents)
                        {
                            _unreadReceivedEvents.Enqueue(ievent);
                        }
                        eventQueue.AckEvent(ievent);
                    }
                    
                    _networkManager.SendEventFakingLatencyAndPacketLoss(new EventBuilder(ievent).SetAck(true).Build());
                }
            }
            else
            {
                if (ievent.GetTimeoutType().Equals(EventTimeoutTypeEnum.Unreliable))
                {
                    lock (_unreadReceivedEvents)
                    {
                        _unreadReceivedEvents.Enqueue(ievent);
                    }
                }
                else
                {
                    throw new Exception("No such timeout type " + ievent.GetTimeoutType());
                }
            }
        }

        public void ConnectToServer(IPEndPoint serverEndPoint)
        {
            Debug.Log("Connecting to server.");
            int serverId = _networkManager.AddConnection(serverEndPoint);
            EventAction connectionAction = new ConnectionAction();
            EventBuilder eventBuilder = _networkManager.AddNetworkInfo(new EventBuilder(), serverId);
            Event connectionEvent = eventBuilder.SetAck(false)
                                                .SetClientId(serverId)
                                                .SetTimeoutType(connectionAction.GetTimeoutType())
                                                .SetEventType(connectionAction.GetEventType())
                                                .SetPayload(connectionAction)
                                                .Build();
            _networkManager.SendEventFakingLatencyAndPacketLoss(connectionEvent);
            
            AddEventToReliableQueue(connectionEvent);
        }

        public bool GetNextPendingEvent(out Event iEvent)
        {
            lock (_unreadReceivedEvents)
            {
                iEvent = null;
                if (_unreadReceivedEvents.Count <= 0) return false;
                iEvent = _unreadReceivedEvents.Dequeue();
                return true;
            }
        }

        public void ConfirmConnection(int serverId)
        {
            if (_initializationAction != null)
            {
                _initializationAction(serverId);
            }
        }

        public void Disable()
        {
            foreach (KeyValuePair<EventTimeoutTypeEnum,ReliableEventQueue> eventQueue in _sendingEventsQueues)
            {
                eventQueue.Value.Disable();
            }
            _networkManager.Disable();
        }
    }
}