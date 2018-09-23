using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPChannel
{

    Socket sending_socket;
    IPEndPoint sendingEndPoint, remoteEndPoint;
    UdpClient listener;
    Thread listenThread;

    // Creates an UDP sender.
    public UDPChannel(String targetIP, int targetPort)
    {
        SetUpSender(targetIP, targetPort);
    }

    // Creates an UDP receiver.
    public UDPChannel(int listeningPort, Action<Byte[], IPEndPoint> receiveAction)
    {
        SetUpListener(listeningPort, receiveAction);
    }

    // Creates an UDP sender and receiver.
    public UDPChannel(String targetIP, int targetPort, int listeningPort, Action<Byte[], IPEndPoint> receiveAction)
    {
        SetUpSender(targetIP, targetPort);
        SetUpListener(listeningPort, receiveAction);
    }

    private void SetUpSender(String targetIP, int targetPort)
    {
        sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sendingEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
    }

    private void SetUpListener(int listeningPort, Action<Byte[], IPEndPoint> receiveAction)
    {
        listener = new UdpClient(listeningPort);
        remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        ListenForConnections(receiveAction);
    }

    private void ListenForConnections(Action<Byte[], IPEndPoint> receiveAction)
    {
        listenThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    Byte[] receivedBytes = listener.Receive(ref remoteEndPoint);
                    receiveAction(receivedBytes, remoteEndPoint);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        });
        listenThread.Start();
    }

    public void SendTo(byte[] buffer, IPEndPoint endPoint)
    {
        listener.Send(buffer, buffer.Length, endPoint.Address.ToString(), endPoint.Port);
    }

    public void SendMessage(byte[] buffer)
    {
        if (sending_socket == null)
            throw new Exception("Socket has not been initialized.");
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