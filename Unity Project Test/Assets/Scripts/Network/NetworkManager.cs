using System;
using System.Collections.Generic;
using System.Net;
using Libs;
using Network.Events;
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
            private GameObject _gameObject;
            private UDPChannel _sendingChannel;
            private EventManager _eventManager;

            public Channel(IPAddress address, GameObject gameObject, UDPChannel sendingChannel, EventManager eventManager)
            {
                _address = address;
                _gameObject = gameObject;
                _sendingChannel = sendingChannel;
                _eventManager = eventManager;
            }

            public IPAddress Address
            {
                get { return _address; }
                set { _address = value; }
            }

            public GameObject GameObject
            {
                get { return _gameObject; }
                set { _gameObject = value; }
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
        }

        void ListenAction(UDPChannel udpChannel, Byte[] receivedBytes)
        {
            BitBuffer bitBuffer = new BitBuffer(receivedBytes);
            IPAddress remoteAddress = udpChannel.GetRemoteIpAddress();
            Channel channel;
            IEvent iEvent;
            
            if (_channels.ContainsKey(remoteAddress))
            {
                channel = _channels[remoteAddress];
            }
            else
            {
                List<IEvent> iEvents = new List<IEvent>();
                EventManager eventManager = new EventManager();
                do
                {
                    iEvent = eventManager.readEvent(bitBuffer);
                    //TODO GET SEQ ID (IGNORE SEQID if Snapshot)
                    if (iEvent == null)
                    {
                        /*TODO Elaborate Exception (Not created exception received)
                               OR IGNORE everything else until Create Exception*/
                        //throw new NotFiniteNumberException();
                    }
                    else
                    {
                        iEvents.Add(iEvent);
                    }

                } while (!(iEvent is CreationEvent));

                iEvents.RemoveAt(iEvents.IndexOf(iEvent));
                iEvent.Process(null);
                GameObject gameObject = ((CreationEvent) iEvent).GameObject;
                UnityEngine.Object.Instantiate(gameObject);
                foreach (IEvent iEventt in iEvents)
                {
                    iEventt.Process(gameObject);
                }

                channel = new Channel(remoteAddress,gameObject,udpChannel,eventManager);
                _channels.Add(remoteAddress,channel);
            }

            do
            {
                iEvent = channel.ChannelEventManager.readEvent(bitBuffer);
                if (iEvent != null)
                {
                    //TODO GET SEQ ID (IGNORE SEQID if Snapshot)
                    iEvent.Process(channel.GameObject);
                }
            }while (iEvent != null);
            //TODO Send ACKs, Deliver Events to other Channels
        }
    }
}