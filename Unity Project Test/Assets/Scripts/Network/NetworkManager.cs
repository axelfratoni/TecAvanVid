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
        private class Client
        {
            private int id;
            private IPEndPoint ip;
            private int last_seq_id = 0;
            private object data;
            private UDPChannel sendingChannel;
        }

        private List<Client> _clients;

        public void addClient()
        {
            Client newClient = new Client();
            _clients.Add(newClient);
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
        
        IEvent readEvent(BitBuffer bitBuffer)
        {
            //Read id
            int id = bitBuffer.readInt(1, 5);
            //Read data
            EventTimeoutTypeEnum eventTimeoutType = EventManager.GetEventTimeoutType((EventEnum)id);
            float y = bitBuffer.readFloat(0.0f, 3.0f, 0.1f);
            float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
            Vector3 newPosition = new Vector3(1, y, z + 1);

            float time = bitBuffer.readFloat(0.0f, 3600.0f, 0.01f);
            
            //networkBuffer.AddItem(newPosition, time);
            //networkBuffer2.AddItem(newPosition, time);

            return null;
        }
        
        void ListenAction(Byte[] receivedBytes)
        {
            BitBuffer bitBuffer = new BitBuffer(receivedBytes);
            
            readSnapshot(bitBuffer);


        }
        
        public NetworkManager(int port)
        {
            new UDPChannel(11001, ListenAction);
        }
    }
}