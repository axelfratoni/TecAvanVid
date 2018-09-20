using System;
using System.Collections.Generic;
using System.Net;
using Libs;
using Network;
using Network.Events;
using UnityEngine;

public class NetworkManager
{
    // final variable to manage being server or not
    private readonly bool _isServer;
    // Get record of ids in local enviroment
    private static int _localLastId = 1;

    // Get reference of server initial Channel
    private readonly UDPChannel _initialServerUdpChannel;

    // List of channels sorted by IP
    private readonly SortedList<IPAddress,Channel> _channels = new SortedList<IPAddress, Channel>();
    // List of local BallEric, referenced by ID
    //                           id Object
    // TODO List of balls must be placed in GameManager
    private readonly SortedList<int,BallEric> _balls = new SortedList<int, BallEric>();
    
    
    /***************************
     TODO CHANGE IDS TO BE UNIQUE
     Client -> [Creation Event (ClientId)] -> Server
     Server -> [Creation Event (ClientId,ObjId)] -> Client
     
     TODO
     LOST IMPLEMETION OF LOCAL/REMOTE BALL
     it's local if has own clientId, it's remote otherwise
     ***************************/
        
    //Server Constructor
    public NetworkManager(int port)
    {
        //Set listening channel
        _initialServerUdpChannel = new UDPChannel(port, ListenAction);
        _isServer = true;
    }
        
    //Client Constructor
    public NetworkManager(int localPort, IPAddress remoteAddress, int remotePort)
    {
        _isServer = false;
        //Get new channel and add it to (should be) unique connection
        UDPChannel udpChannel = new UDPChannel(remoteAddress.ToString(), remotePort, localPort, ListenAction);
        _channels.Add(remoteAddress,new Channel(remoteAddress, udpChannel, new EventManager(), _isServer));
    }

    //Inner class of Channel
    private class Channel
    {
        // Addres from
        private IPAddress _address;
        //Actual UDP Channel
        private UDPChannel _sendingChannel;
        //Channel's Event Manager
        private readonly EventManager _eventManager;
        // Server reference to assing ids
        private readonly bool _imServer;

        public Channel(IPAddress address, UDPChannel sendingChannel, EventManager eventManager, bool imServer)
        {
            _address = address;
            _sendingChannel = sendingChannel;
            _eventManager = eventManager;
            _imServer = imServer;
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

        public void Disable()
        {
            _sendingChannel.Disable();
        }
    }

    //Callback for new data
    void ListenAction(UDPChannel udpChannel, Byte[] receivedBytes)
    {
        //Transform to bitbuffer
        BitBuffer bitBuffer = new BitBuffer(receivedBytes);
        IPAddress remoteAddress = udpChannel.GetRemoteIpAddress();
        Channel channel;
        IEvent iEvent;
        
        //This list get which timeout List to send ack
        SortedList<EventTimeoutTypeEnum,bool> updatedAck = new SortedList<EventTimeoutTypeEnum , bool>();

        foreach (EventTimeoutTypeEnum eventTimeoutType in Enum.GetValues(typeof(EventTimeoutTypeEnum)))
        {
            updatedAck.Add(eventTimeoutType,false);
        }
        
        //Check if its a new connection
        if (_channels.ContainsKey(remoteAddress))
        {
            //Its a known connection
            channel = _channels[remoteAddress];
        }
        else
        {
            //Its a not known connection
            if (!_isServer)
            {
                //If i'm not server, should ignore
                return;
            }
            // Builds a new Channel
            EventManager eventManager = new EventManager();
            channel = new Channel(remoteAddress,udpChannel,eventManager,_isServer);
            // Set the channel bidirectional
            udpChannel.SetSenderFromListener();
            // As i'm server and have a new client, notify of all already existing objects
            foreach (int localId in _balls.Keys)
            {
                //TODO CHANGE TO UNIQUE ID channel.AddBallId(localId);
                channel.ChannelEventManager.addEvent(new CreationEvent(0,localId));
            }
            // Temp IEvent list up to Creation Event Received
            List<IEvent> iEvents = new List<IEvent>();
            do
            {
                //Read Event
                iEvent = eventManager.readEvent(bitBuffer);
                //If no event, returns null
                if (iEvent == null)
                {
                    //And exit
                    return;
                }
                //Save temporaly
                iEvents.Add(iEvent);
            } while (!(iEvent is CreationEvent)); //Until its a creation event
            
            //remove CreationEvent from temp List
            iEvents.RemoveAt(iEvents.IndexOf(iEvent));
            //Process it (actually create the BallEric Object), and broadcast
            ProcessEvent(iEvent,channel,updatedAck);
            
            //Get Ball
            BallEric ball = ((CreationEvent) iEvent).Ball;
            //Get new local Id to assign
            int newLocalId = GetNewLocalId();
            //And add it to the list
            _balls.Add(newLocalId,ball);
            
            //TODO CHECK if shows it
            //Instantiate it
            //UnityEngine.Object.Instantiate(gameObject);
            
            // Add channel to list
            _channels.Add(remoteAddress,channel);
            
            //Process saved Ievents
            foreach (IEvent iEventt in iEvents)
            {
                ProcessEvent(iEventt, channel, updatedAck);
            }
        }
        
        //Process remain events
        do
        {
            iEvent = channel.ChannelEventManager.readEvent(bitBuffer);
            ProcessEvent(iEvent, channel, updatedAck);
        }while (iEvent != null);
        //For each timeout stack
        foreach (EventTimeoutTypeEnum eventTimeoutType in Enum.GetValues(typeof(EventTimeoutTypeEnum)))
        {
            //Check if have to update
            if (updatedAck[eventTimeoutType])
            {
                channel.ChannelEventManager.addEvent(new ACKEvent(eventTimeoutType,channel.ChannelEventManager.GetLastACK(eventTimeoutType)));
            }
        }
    }

    void ProcessEvent(IEvent iEvent, Channel channel, SortedList<EventTimeoutTypeEnum,bool> updatedAck)
    {
        if (iEvent != null)
        {
            //If its an ACK, update list to send
            if (iEvent.GetEventEnum() == EventEnum.ACK)
            {
                channel.ChannelEventManager.clearEventList((ACKEvent)iEvent);
            }
            //If its an snapshot, update and Broadcast it
            else if (iEvent.GetEventEnum() == EventEnum.Snapshot)
            {
                iEvent.Process(_balls[channel.GetBallLocalId(iEvent.Id)]);
                BroadcastEvent(channel,iEvent);
            }
            //Every else, should check sequence to process it
            else
            {
                int seqId = iEvent.SeqId;
                EventTimeoutTypeEnum timeoutType = EventManager.GetEventTimeoutType(iEvent.GetEventEnum());
                int lastAck = channel.ChannelEventManager.GetLastACK(timeoutType);
                if (seqId > lastAck || (seqId == lastAck && updatedAck[timeoutType]))
                {
                    updatedAck[timeoutType] = true;
                    channel.ChannelEventManager.SetLastACK(timeoutType, seqId);
                    if (iEvent.GetEventEnum() == EventEnum.EventCreation)
                    {
                        iEvent.Process(null);
                    }
                    else
                    {
                        iEvent.Process(_balls[channel.GetBallLocalId(iEvent.Id)]);
                        
                    }
                    BroadcastEvent(channel,iEvent);
                }
            }
        }
    }

    void BroadcastEvent(Channel sendingChannel, IEvent iEvent)
    {
        foreach (Channel cchannel in _channels.Values)
        {
            //Add the event
            sendingChannel.ChannelEventManager.addEvent(iEvent);
        }
    }

    public void Disable()
    {
        if (_initialServerUdpChannel != null)
        {
            _initialServerUdpChannel.Disable();
        }

        foreach (Channel channel in _channels.Values)
        {
            channel.Disable();
        }
    }

    public void AddEvent(IEvent iEvent, int localId)
    {
        foreach (Channel channel in _channels.Values)
        {
            channel.ChannelEventManager.addEvent(iEvent);
        }
    }

    public bool IsServer
    {
        get { return _isServer; }
    }

    public static int GetNewLocalId()
    {
        return _localLastId++;
    }

    public void Loop()
    {
            
    }
}