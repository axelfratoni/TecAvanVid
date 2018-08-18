using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Libs;
using UnityEngine;

public class UDPClient : MonoBehaviour {

    Socket sending_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse("10.17.68.211"), 11000);
    //IPEndPoint sending_end_point = new IPEndPoint(IPAddress.Parse("10.17.68.213"), 11000);
    BitBuffer bitBuffer = new BitBuffer(1024);

    private float time;
    private float ctime;
    private static float cycleTime = 0.1f;
    // Use this for initialization
	void Start ()
	{
	    time = Time.deltaTime;
	    ctime = Time.deltaTime;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    ctime += Time.deltaTime;
	    time += Time.deltaTime; 
        if (ctime > cycleTime)
        {
            Debug.Log("Send" + time);
            SendMessage();
            ctime -= cycleTime;
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

            //Debug.Log("Cli Pos: " + x + " " + y + " " + z + " ");
            //Debug.Log("Cli Quaternion: " + qx + " " + qy + " " + qz + " " + qw + " ");
            /*bitBuffer.writeInt(33,0,255); // !
            bitBuffer.writeInt(40,0,255); // (
            bitBuffer.writeInt(41,0,255); // )
            bitBuffer.writeInt(64,0,255); // @
            bitBuffer.writeInt(72,0,255); // H*/
            bitBuffer.writeFloat(-x*(-1),-31.0f,31.0f,0.1f);
            bitBuffer.writeFloat(y,-1.0f,15.0f,0.1f);
            bitBuffer.writeFloat(z-1+1,-31.0f,31.0f,0.1f);
            bitBuffer.writeFloat(qx,-1.0f,1.0f,0.01f);
            bitBuffer.writeFloat(qy,-1.0f,1.0f,0.01f);
            bitBuffer.writeFloat(qz,-1.0f,1.0f,0.01f);
            bitBuffer.writeFloat(qw,-1.0f,1.0f,0.01f);
            bitBuffer.writeFloat(time,0.0f,3600.0f,0.01f);
            bitBuffer.flush();
            sending_socket.SendTo(bitBuffer.getBuffer(), sending_end_point);
        }
        catch (Exception socketException)
        {
            Debug.Log("Socket exception: " + socketException);
        }
    }

    private void OnDisable()
    {
        sending_socket.Close();
    }
}
