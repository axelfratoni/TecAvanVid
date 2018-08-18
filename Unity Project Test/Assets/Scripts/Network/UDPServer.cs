using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

public class UDPServer {

	UdpClient listener;
    IPEndPoint anyClient;
	Thread listenThread;

    public UDPServer(int port, Action<Byte[]> receiveAction)
    {
        listener = new UdpClient(port);
        anyClient = new IPEndPoint(IPAddress.Any, 0);
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
					Byte[] receivedBytes = listener.Receive(ref anyClient);
                    receiveAction(receivedBytes);
				}
				catch (Exception e)
				{
					Debug.Log(e.ToString());
				}
			}
		});
		listenThread.Start();
	}

	public void Disable()
	{
		listenThread.Abort();
		listener.Close();
	}
}