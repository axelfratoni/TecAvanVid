using Libs;
using System;
using System.Collections;
using UnityEngine;

public class NetworkBall : MonoBehaviour {

    public int listeningPort = 11000;

    private UDPChannel udpReceiver;
    private NetworkBuffer<Vector3> networkBuffer;
    private NetworkBuffer2<Vector3> networkBuffer2;


    void Start () {
        udpReceiver = new UDPChannel(listeningPort, ListenAction);
        networkBuffer = new NetworkBuffer<Vector3>();
        networkBuffer2 = new NetworkBuffer2<Vector3>();
    }

    void ListenAction(Byte[] receivedBytes)
    {
        BitBuffer bitBuffer = new BitBuffer(receivedBytes);

        float x = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        float y = bitBuffer.readFloat(0.0f, 3.0f, 0.1f);
        float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        Vector3 newPosition = new Vector3(x + 1, y, z + 1);

        float time = bitBuffer.readFloat(0.0f, 3600.0f, 0.01f);

        networkBuffer.AddItem(newPosition, time);
        networkBuffer2.AddItem(newPosition, time);
    }
	
	void Update () {
        UpdatePositionAxel();
        //UpdatePositionEric();
    }

    private void UpdatePositionEric()
    {
        NetworkBuffer<Vector3>.InterpolatedItem<Vector3> interpolatedItem = networkBuffer.GetNextItem();
        if (interpolatedItem != null)
        {
            transform.position = Vector3.Lerp(interpolatedItem.previous, interpolatedItem.next, interpolatedItem.interpolation);
        }
    }

    private void UpdatePositionAxel()
    {
        NetworkBuffer2<Vector3>.InterpolatedItem<Vector3> interpolatedItem = networkBuffer2.GetNextItem();
        if (interpolatedItem != null)
        {
            transform.position = Vector3.Lerp(interpolatedItem.previous, interpolatedItem.next, interpolatedItem.interpolation);
        }
    }

    private void OnDisable()
    {
        udpReceiver.Disable();
    }
}
