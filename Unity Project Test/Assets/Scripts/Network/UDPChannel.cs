using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPChannel
{

    Socket sending_socket;
    IPEndPoint sendingEndPoint, listeningEndPoint;
    UdpClient listener;
    Thread listenThread;

    // Creates an UDP sender.
    public UDPChannel(String targetIP, int targetPort)
    {
        SetUpSender(targetIP, targetPort);
    }

    // Creates an UDP receiver.
    public UDPChannel(int listeningPort, Action<Byte[]> receiveAction)
    {
        SetUpListener(listeningPort, receiveAction);
    } 

    // Creates an UDP sender and receiver.
    public UDPChannel(String targetIP, int targetPort, int listeningPort, Action<Byte[]> receiveAction)
    {
        SetUpSender(targetIP, targetPort);
        SetUpListener(listeningPort, receiveAction);
    }
    
    // Creates an UDP sender and receiver from sender
    public UDPChannel(UDPChannel original, int listeningPort, Action<Byte[]> receiveAction)
    {
        sending_socket = original.sending_socket;
        sendingEndPoint = original.sendingEndPoint;
        SetUpListener(listeningPort, receiveAction);
    }
    
    // Creates an UDP sender and receiver from receiver
    public UDPChannel(UDPChannel original, String targetIP, int targetPort)
    {
        SetUpSender(targetIP, targetPort);
        listener = original.listener;
        listenThread = original.listenThread;
    }

    private void SetUpSender(String targetIP, int targetPort)
    {
        sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sendingEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
    }

    private void SetUpListener(int listeningPort, Action<Byte[]> receiveAction)
    {
        listener = new UdpClient(listeningPort);
        listeningEndPoint = new IPEndPoint(IPAddress.Any, 0);
        ListenForConnections(receiveAction);
    }

    private void ListenForConnections(Action<Byte[]> receiveAction)
    {
        listenThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    Byte[] receivedBytes = listener.Receive(ref listeningEndPoint);
                    receiveAction(receivedBytes);
                    int a = ((IPEndPoint)(listener.Client.RemoteEndPoint)).Port;
                    Debug.Log(a);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        });
        listenThread.Start();
    }

    public void SendMessage(byte[] buffer)
    {
        if (sending_socket == null)
            return;
        try
        {
            sending_socket.SendTo(buffer, sendingEndPoint);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendMessage(byte[] buffer, String targetIP, int targetPort)
    {
        SetUpSender(targetIP, targetPort);
        SendMessage(buffer);
    }

    public void Disable()
    {
        if(sending_socket != null)
            sending_socket.Close();
        if(listenThread != null)
            listenThread.Abort();
        if(listener != null)
            listener.Close();
    }
}