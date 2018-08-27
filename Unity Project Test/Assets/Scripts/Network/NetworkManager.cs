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
        private class Channel
        {
            private int id;
            private int last_seq_id = 0;
            private object data;
            private UDPChannel sendingChannel;

        }

        private SortedList<IPEndPoint,Channel> _channels;

        public void AddClient()
        {
            _channels.Add(null,new Channel());
        }

        void readSnapshot(BitBuffer bitBuffer)
        {
            
            float x = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
            float y = bitBuffer.readFloat(0.0f, 3.0f, 0.1f);
            float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
            Vector3 newPosition = new Vector3(x + 1, y, z + 1);

            float time = bitBuffer.readFloat(0.0f, 3600.0f, 0.01f);
            
            //networkBuffer.AddItem(newPosition, time);
            //networkBuffer2.AddItem(newPosition, time);
        }
        
        void ListenAction(Byte[] receivedBytes)
        {
            BitBuffer bitBuffer = new BitBuffer(receivedBytes);
            
            readSnapshot(bitBuffer);
            IEvent iEvent;
            do
            {
                iEvent = EventManager.GetInstance().readEvent(bitBuffer);
                iEvent.process();
            }while (iEvent != null);


        }
        
        public NetworkManager(int port)
        {
            new UDPChannel(11001, ListenAction);
            _channels = new SortedList<IPEndPoint, Channel>();
        }
    }
}