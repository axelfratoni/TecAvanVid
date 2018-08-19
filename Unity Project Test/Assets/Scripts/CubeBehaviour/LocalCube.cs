using Libs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCube : MonoBehaviour {

    public string targetIP = "127.0.0.1";
    public int targetPort = 11000;

    private UDPChannel udpSender;
    private BitBuffer bitBuffer;
    private float time;
    private float ctime;
    private static float cycleTime = 0.1f;

    void Start()
    {
        udpSender = new UDPChannel(targetIP, targetPort);
        bitBuffer = new BitBuffer(1024);
        time = Time.deltaTime;
        ctime = Time.deltaTime;
    }

    void Update()
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
        var x = transform.position.x;
        var y = transform.position.y;
        var z = transform.position.z;
        var qx = transform.rotation.x;
        var qy = transform.rotation.y;
        var qz = transform.rotation.z;
        var qw = transform.rotation.w;

        //Debug.Log("Cli Pos: " + x + " " + y + " " + z + " ");
        //Debug.Log("Cli Quaternion: " + qx + " " + qy + " " + qz + " " + qw + " ");
        /*bitBuffer.writeInt(33,0,255); // !
        bitBuffer.writeInt(40,0,255); // (
        bitBuffer.writeInt(41,0,255); // )
        bitBuffer.writeInt(64,0,255); // @
        bitBuffer.writeInt(72,0,255); // H*/
        bitBuffer.writeFloat(-x * (-1), -31.0f, 31.0f, 0.1f);
        bitBuffer.writeFloat(y, -1.0f, 15.0f, 0.1f);
        bitBuffer.writeFloat(z - 1 + 1, -31.0f, 31.0f, 0.1f);
        bitBuffer.writeFloat(qx, -1.0f, 1.0f, 0.01f);
        bitBuffer.writeFloat(qy, -1.0f, 1.0f, 0.01f);
        bitBuffer.writeFloat(qz, -1.0f, 1.0f, 0.01f);
        bitBuffer.writeFloat(qw, -1.0f, 1.0f, 0.01f);
        bitBuffer.writeFloat(time, 0.0f, 3600.0f, 0.01f);
        bitBuffer.flush();
        udpSender.SendMessage(bitBuffer.getBuffer());
    }

    private void OnDisable()
    {
        udpSender.Disable();
    }
}
