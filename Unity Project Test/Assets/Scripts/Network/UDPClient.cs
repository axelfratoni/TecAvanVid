using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class UDPClient {

    Socket sending_socket;
    IPEndPoint sending_end_point;

    public UDPClient(String targetIP, int targetPort)
    {
        sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        sending_end_point = new IPEndPoint(IPAddress.Parse(targetIP), targetPort);
    }
	
    public void SendMessage(byte[] buffer)
    {
        try
        {
            sending_socket.SendTo(buffer, sending_end_point);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    public void Disable()
    {
        sending_socket.Close();
    }
}
