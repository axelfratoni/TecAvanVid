using System;
using System.Collections.Generic;
using System.Net;
using Libs;
using Network.Events;
using UnityEditor;
using UnityEngine;

namespace Network
{
    public class NetworkManager
    {
        private SortedList<IPAddress,Channel> _channels;

        public NetworkManager(int port)
        {
            new UDPChannel(11001, ListenAction);
            _channels = new SortedList<IPAddress, Channel>();
        }

        private class Channel
        {
            private IPAddress _address;
            private SortedList<int,GameObject> _gameObjects;
            private UDPChannel _sendingChannel;
            private EventManager _eventManager;

            public Channel(IPAddress address, UDPChannel sendingChannel, EventManager eventManager)
            {
                _address = address;
                _gameObjects = new SortedList<int, GameObject>();
                _sendingChannel = sendingChannel;
                _eventManager = eventManager;
            }

            public IPAddress Address
            {
                get { return _address; }
                set { _address = value; }
            }

            public UDPChannel SendingChannel
            {
                get { return _sendingChannel; }
                set { _sendingChannel = value; }
            }

            public EventManager ChannelEventManager
            {
                get { return _eventManager; }
            }

            public void AddGameObject(int id, GameObject gameObject)
            {
                _gameObjects.Add(id,gameObject);
            }
            
            public GameObject GetGameObject(int id)
            {
                return _gameObjects[id];
            }
        }

        void ListenAction(UDPChannel udpChannel, Byte[] receivedBytes)
        {
            BitBuffer bitBuffer = new BitBuffer(receivedBytes);
            IPAddress remoteAddress = udpChannel.GetRemoteIpAddress();
            Channel channel;
            IEvent iEvent;
            
            SortedList<EventTimeoutTypeEnum,bool> updatedACK = new SortedList<EventTimeoutTypeEnum , bool>();

            foreach (EventTimeoutTypeEnum eventTimeoutType in Enum.GetValues(typeof(EventTimeoutTypeEnum)))
            {
                updatedACK.Add(eventTimeoutType,false);
            }
            
            if (_channels.ContainsKey(remoteAddress))
            {
                channel = _channels[remoteAddress];
            }
            else
            {
                List<IEvent> iEvents = new List<IEvent>();
                EventManager eventManager = new EventManager();
                channel = new Channel(remoteAddress,udpChannel,eventManager);
                do
                {
                    iEvent = eventManager.readEvent(bitBuffer);
                    if (iEvent == null)
                    {
                        return;
                    }
                    else
                    {
                        iEvents.Add(iEvent);
                    }

                } while (!(iEvent is CreationEvent));

                iEvents.RemoveAt(iEvents.IndexOf(iEvent));
                iEvent.Process(null);
                GameObject gameObject = ((CreationEvent) iEvent).GameObject;
                channel.AddGameObject(iEvent.GetId(),gameObject);
                UnityEngine.Object.Instantiate(gameObject);
                channel.ChannelEventManager.addEvent(iEvent);
                _channels.Add(remoteAddress,channel);

                foreach (IEvent iEventt in iEvents)
                {
                    processEvent(iEvent, channel, updatedACK);
                }
            }
            
            do
            {
                iEvent = channel.ChannelEventManager.readEvent(bitBuffer);
                processEvent(iEvent, channel, updatedACK);
            }while (iEvent != null);
            foreach (EventTimeoutTypeEnum eventTimeoutType in Enum.GetValues(typeof(EventTimeoutTypeEnum)))
            {
                if (updatedACK[eventTimeoutType])
                {
                    channel.ChannelEventManager.addEvent(new ACKEvent(eventTimeoutType,channel.ChannelEventManager.GetLastACK(eventTimeoutType)));
                }
            }
        }

        void processEvent(IEvent iEvent, Channel channel, SortedList<EventTimeoutTypeEnum,bool> updatedACK)
        {
            if (iEvent != null)
            {
                if (iEvent.GetEventEnum() == EventEnum.ACK)
                {
                    channel.ChannelEventManager.clearEventList((ACKEvent)iEvent);
                }else if (iEvent.GetEventEnum() == EventEnum.Snapshot)
                {
                    iEvent.Process(channel.GetGameObject(iEvent.GetId()));
                }
                else
                {
                    int seqId = iEvent.GetSeqId();
                    EventTimeoutTypeEnum timeoutType = EventManager.GetEventTimeoutType(iEvent.GetEventEnum());
                    if (seqId >= channel.ChannelEventManager.GetLastACK(timeoutType))
                    {
                        updatedACK[timeoutType] = true;
                        channel.ChannelEventManager.SetLastACK(timeoutType, seqId);
                        iEvent.Process(channel.GetGameObject(iEvent.GetId()));
                        foreach (Channel cchannel in _channels.Values)
                        {
                            if (cchannel != channel)
                            {
                                channel.ChannelEventManager.addEvent(iEvent);
                            }
                        }
                    }
                }
            }
        }
    }
}