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
        private readonly ActionDispatcher _actionDispatcher;
        private readonly Dictionary<EventTimeoutTypeEnum, ReliableEventQueue> _eventQueues;
        
        public EventManager(ActionDispatcher actionDispatcher, int localPort)
        {
            _actionDispatcher = actionDispatcher;
            _networkManager = new NetworkManager(localPort, this);
            _eventQueues = new Dictionary<EventTimeoutTypeEnum, ReliableEventQueue>
            {
                {EventTimeoutTypeEnum.NoTimeOut, new ReliableEventQueue(5, SendEventsInQueue)},
                {EventTimeoutTypeEnum.TimeOut, new ReliableEventQueue(1000, SendEventsInQueue)}
            };
        }

        private void SendEventsInQueue(List<Event> eventQueue)
        {
            eventQueue.ForEach(ev =>
            {
                _networkManager.SendEvent(ev);
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
            _networkManager.SendEvent(ievent);

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
                if (_eventQueues.TryGetValue(ievent.GetTimeoutType(), out eventQueue))
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
            if (_eventQueues.TryGetValue(ievent.GetTimeoutType(), out eventQueue))
            {
                if (ievent.Ack)
                {
                    eventQueue.AckEvent(ievent);
                }
                else
                {
                    if (eventQueue.ShouldProcessEvent(ievent) && ievent.GetPayload() != null)
                    {
                        ievent.Execute(_actionDispatcher);
                    }
                    
                    _networkManager.SendEvent(new EventBuilder(ievent).SetAck(true).Build());
                }
            }
            else
            {
                if (ievent.GetTimeoutType().Equals(EventTimeoutTypeEnum.Unreliable))
                {
                    ievent.Execute(_actionDispatcher);
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
            _networkManager.SendEvent(connectionEvent);
            
            AddEventToReliableQueue(connectionEvent);
        }

        public void ConfirmConnection(int serverId)
        {
            _actionDispatcher.InitializeGame(serverId);
        }

        public void Disable()
        {
            foreach (KeyValuePair<EventTimeoutTypeEnum,ReliableEventQueue> eventQueue in _eventQueues)
            {
                eventQueue.Value.Disable();
            }
            _networkManager.Disable();
        }
    }
}