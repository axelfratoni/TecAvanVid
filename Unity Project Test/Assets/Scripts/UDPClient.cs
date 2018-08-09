using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class UDPClient : MonoBehaviour {

    Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);

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
            sending_socket.SendTo(send_buffer, sending_end_point);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }
}
