using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Libs;
using UnityEngine;

public class UDPClient : MonoBehaviour {

    Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
    BitBuffer bitBuffer = new BitBuffer(1024);

    // Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Key A down");
            SendMessage();
        }
    }

    private void SendMessage()
    {
        string clientMessage = "This is a message from client.";
        byte[] send_buffer = Encoding.ASCII.GetBytes(clientMessage);
        try
        {
            bitBuffer.writeInt(422,350,750);
            bitBuffer.writeInt(422,350,750);
            bitBuffer.writeInt(422,350,750);
            bitBuffer.flush();
            sending_socket.SendTo(bitBuffer.getBuffer(), sending_end_point);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}
