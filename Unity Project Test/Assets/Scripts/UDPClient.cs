using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Libs;
using UnityEngine;

public class UDPClient : MonoBehaviour {

    Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
    BitBuffer bitBuffer = new BitBuffer(1024*4);

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
        try
        {
            var x = GameObject.Find("Cube").transform.position.x;
            var y = GameObject.Find("Cube").transform.position.y;
            var z = GameObject.Find("Cube").transform.position.z;
            var qx = GameObject.Find("Cube").transform.rotation.x;
            var qy = GameObject.Find("Cube").transform.rotation.y;
            var qz = GameObject.Find("Cube").transform.rotation.z;
            var qw = GameObject.Find("Cube").transform.rotation.w;
            
            Debug.Log("Quaternion: " + qx + " " + qy + " " + qz + " " + qw + " ");
            bitBuffer.writeFloat(x,-31.0f,31.0f,1f);
            bitBuffer.writeFloat(y,0.0f,3.0f,1f);
            bitBuffer.writeFloat(z,-31.0f,31.0f,1f);
            bitBuffer.writeFloat(qx,0.0f,1.0f,0.01f);
            bitBuffer.writeFloat(qy,0.0f,1.0f,0.01f);
            bitBuffer.writeFloat(qz,0.0f,1.0f,0.01f);
            bitBuffer.writeFloat(qw,0.0f,1.0f,0.01f);
            bitBuffer.flush();
            sending_socket.SendTo(bitBuffer.getBuffer(), sending_end_point);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

}
