using Libs;
using System;
using System.Collections;
using UnityEngine;

public class NetworkBall : MonoBehaviour {

    private UDPChannel udpReceiver;
    private SortedList sortedList;
    private readonly int MAX_QUEUE_SIZE = 5;
    private readonly System.Object lockThis = new System.Object();
    private float previousTime;
    private float acumTime;

    void Start () {
        udpReceiver = new UDPChannel(11000, ListenAction);
        sortedList = new SortedList();
        previousTime = 0;
        acumTime = 0;
    }

    void ListenAction(Byte[] receivedBytes)
    {
        BitBuffer bitBuffer = new BitBuffer(receivedBytes);

        float x = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        float y = bitBuffer.readFloat(0.0f, 3.0f, 0.1f);
        float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        Vector3 newPosition = new Vector3(x + 1, y, z + 1);

        float time = bitBuffer.readFloat(0.0f, 3600.0f, 0.01f);

        lock (lockThis)
        {
            if (sortedList.Count > 0 && time < (float)sortedList.GetKey(sortedList.Count - 1))
                return;

            sortedList.Add(time, newPosition);
            if(sortedList.Count > MAX_QUEUE_SIZE)
            {
                sortedList.RemoveAt(0);
            }
        }
    }
	
	void Update () {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (sortedList.Count > 2)
        {
            float newPreviousTime = (float)sortedList.GetKey(0);
            if (previousTime < newPreviousTime)
            {
                previousTime = newPreviousTime;
                acumTime = 0;
            }
            float window = (float)sortedList.GetKey(1) - (float)sortedList.GetKey(0);
            if(acumTime > window)
            {
                sortedList.RemoveAt(0);
                acumTime = 0;
                previousTime = (float)sortedList.GetKey(0);
            }
            window = (float)sortedList.GetKey(1) - (float)sortedList.GetKey(0);
            float moveInterpolation = acumTime / window;

            Vector3 nextMove = (Vector3)sortedList.GetByIndex(1);
            Vector3 previousMove = (Vector3)sortedList.GetByIndex(0);
            Vector3 deltaMove = (nextMove - previousMove) * moveInterpolation;

            transform.position = previousMove + deltaMove;
            acumTime += Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        udpReceiver.Disable();
    }
}
