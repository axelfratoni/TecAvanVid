using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPChannel
{

    private Socket _sendingSocket;
    private IPEndPoint _sendingEndPoint, _remoteEndPoint;
    private UdpClient _listener;
    private Thread _listenThread;

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
        _sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _sendingEndPoint = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
    }

    private void SetUpListener(int listeningPort, Action<Byte[], IPEndPoint> receiveAction)
    {
        _listener = new UdpClient(listeningPort);
        _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
        ListenForConnections(receiveAction);
    }

    private void ListenForConnections(Action<Byte[], IPEndPoint> receiveAction)
    {
        _listenThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    Byte[] receivedBytes = _listener.Receive(ref _remoteEndPoint);
                    receiveAction(receivedBytes, _remoteEndPoint);
                }
                catch (Exception e)
                {
                    Debug.Log(e.ToString());
                }
            }
        });
        _listenThread.Start();
    }

    public void SetUpListenerFromSender(Action<Byte[], IPEndPoint> receiveAction)
    {
        _listener = new UdpClient{Client = _sendingSocket};
        ListenForConnections(receiveAction);
    }

    public void SendMessage(byte[] buffer)
    {
        if (_sendingSocket == null)
            throw new Exception("Socket has not been initialized.");
        try
        {
            _sendingSocket.SendTo(buffer, _sendingEndPoint);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void SendMessageTo(byte[] buffer, IPEndPoint endPoint)
    {
        //_listener.Connect(endPoint);
        _listener.Send(buffer, buffer.Length, endPoint);
    }

    public void SendMessage(byte[] buffer, String targetIP, int targetPort)
    {
        SetUpSender(targetIP, targetPort);
        SendMessage(buffer);
    }

    public void Disable()
    {
        if(_sendingSocket != null)
            _sendingSocket.Close();
        if(_listenThread != null)
            _listenThread.Abort();
        if(_listener != null)
            _listener.Close();
    }

    ~UDPChannel()
    {
        Disable();
    }
}