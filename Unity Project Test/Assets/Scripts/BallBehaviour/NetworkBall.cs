using Libs;
using System;
using System.Collections;
using UnityEngine;

public class NetworkBall : MonoBehaviour {

    public int listeningPort = 11000;

    private UDPChannel udpReceiver;
    private SortedList positionList;
    private NetworkBuffer<Vector3> networkBuffer;
    private float time;

    void Start () {
        udpReceiver = new UDPChannel(listeningPort, ListenAction);
        positionList = new SortedList();
        time = 0;
        networkBuffer = new NetworkBuffer<Vector3>();
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

        if (!(positionList.Count > 3 && time < (float)positionList.GetKey(3)))
        {
            positionList.Add(time, newPosition);
        }
    }
	
	void Update () {
        UpdatePosition();
        //UpdatePositionWithNetworkBuffer();
    }

    private void UpdatePositionWithNetworkBuffer()
    {
        NetworkBuffer<Vector3>.InterpolatedItem<Vector3> interpolatedItem = networkBuffer.GetNextItem();
        if (interpolatedItem != null)
        {
            Vector3 temp = (interpolatedItem.next - interpolatedItem.previous) * interpolatedItem.interpolation;
            transform.position = interpolatedItem.previous + temp;
        }
    }

    private void UpdatePosition()
    {
        if (positionList.Count > 2)
        {
            if(time == 0)
            {
                time = (float)positionList.GetKey(0);
            }
            else
            {
                time += Time.deltaTime;
            }
            if (time > (float)positionList.GetKey(1))
            {
                positionList.RemoveAt(0);
            }

            Vector3 nextPosition = (Vector3)positionList.GetByIndex(1);
            float nextPositionTime = (float)positionList.GetKey(1);
            Vector3 previousPosition = (Vector3)positionList.GetByIndex(0);
            float previousPositionTime = (float)positionList.GetKey(0);
            Vector3 deltaPosition = (nextPosition - previousPosition) * ((time - previousPositionTime) / (nextPositionTime - previousPositionTime));

            transform.position = previousPosition + deltaPosition;
        }
    }

    private void OnDisable()
    {
        udpReceiver.Disable();
    }
}
