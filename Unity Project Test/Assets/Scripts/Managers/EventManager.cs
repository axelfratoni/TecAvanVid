using System;
using System.Collections.Generic;
using System.Net;
using Events.Actions;
using UnityEngine;

namespace Events
{
    public class EventManager
    {
        private readonly GameManager _gameManager;
        private readonly NetworkManager _networkManager;
        private readonly Dictionary<EventTimeoutTypeEnum, ReliableEventQueue> _eventQueues;
        
        public EventManager(GameManager gameManager, int localPort)
        {
            _gameManager = gameManager;
            _networkManager = new NetworkManager(localPort, this);
            _eventQueues = new Dictionary<EventTimeoutTypeEnum, ReliableEventQueue>
            {
                {EventTimeoutTypeEnum.NoTimeOut, new ReliableEventQueue(0, SendEventsInQueue)},
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
                                       .SetPayload(eventAction.Serialize())
                                       .Build();
            _networkManager.SendEvent(ievent);

            AddEventToReliableQueue(ievent);
        }

        public void AddEventToReliableQueue(Event ievent)
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
                    if (eventQueue.ShouldProcessEvent(ievent))
                    {
                        ProcessEvent(ievent);
                    }
                }
            }
            else
            {
                throw new Exception("No such timeout type " + ievent.GetTimeoutType());
            }
            _networkManager.SendEvent(new EventBuilder(ievent).SetAck(true)
                                                               .Build());
        }

        private void ProcessEvent(Event ievent)
        {
            EventAction eventAction = null;
            switch (ievent.GetEventEnum())
            {
                case EventEnum.Connection:
                    break;
                case EventEnum.Snapshot:
                    eventAction = new SnapshotAction(ievent.GetPayload());
                    break;
                case EventEnum.Creation:
                    eventAction = new CreationAction(ievent.GetPayload());
                    break;
                case EventEnum.Movement:
                    eventAction = new MovementAction(ievent.GetPayload());
                    break;
                case EventEnum.Color:
                    eventAction = new ColorAction(ievent.GetPayload());
                    break;
                default:
                    throw new Exception("Invalid event type " + ievent.GetEventEnum());
            }

            if (eventAction != null)
            {
                eventAction.Execute(_gameManager);
            }
        }

        public void ConnectToServer(IPEndPoint serverEndPoint)
        {
            int serverId = _networkManager.AddConnection(serverEndPoint);
            EventBuilder eventBuilder = _networkManager.AddNetworkInfo(new EventBuilder(), serverId);
            Event connectionEvent = eventBuilder.SetAck(false)
                                                .SetClientId(serverId)
                                                .SetTimeoutType(EventTimeoutTypeEnum.TimeOut)
                                                .SetEventType(EventEnum.Connection)
                                                .SetPayload(new byte[0])
                                                .Build();
            _networkManager.SendEvent(connectionEvent);
            
            AddEventToReliableQueue(connectionEvent);
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