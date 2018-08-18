using Libs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetworkBall : MonoBehaviour {

    private UDPServer udpServer;
    private Queue<Move> positionQueue = new Queue<Move>();
    private List<Move> moveList = new List<Move>(5);
    private float lastTime = 0;
    private static float windowTime = 0.1f;

    void Start () {
        udpServer = new UDPServer(11000, ListenAction);
	}

    void ListenAction(Byte[] receivedBytes)
    {
        BitBuffer bitBuffer = new BitBuffer(receivedBytes);
        float x = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        float y = bitBuffer.readFloat(0.0f, 3.0f, 0.1f);
        float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        float time = bitBuffer.readFloat(0.0f, 3600.0f, 0.01f);
        positionQueue.Enqueue(new Move(new Vector3(x + 1, y, z + 1), time));
    }
	
	void Update () {
        while (positionQueue.Count > 0)
        {
            Move m = positionQueue.Dequeue();
            if (moveList.Count == moveList.Capacity)
            {
                moveList.RemoveAt(0);
            }
            moveList.Add(m);
        }
        if (moveList.Count > 3)
        {
            if (lastTime == 0)
            {
                Move m = moveList[0];
                transform.position = m.position;
                lastTime = m.time;
            }
            else
            {
                float time = lastTime + Time.deltaTime;
                Move m;
                Move lastMove;
                int i = 1;
                do
                {
                    m = moveList[i];
                    lastMove = moveList[i - 1];
                    i++;
                } while (m.time < time && i < moveList.Count);

                Vector3 temp = (m.position - lastMove.position) * (time - lastMove.time) / (m.time - lastMove.time);
                transform.position = lastMove.position + temp;
                lastTime += Time.deltaTime;
            }
        }
    }

    private void OnDisable()
    {
        udpServer.Disable();
    }

    private class Move
    {
        public Vector3 position;
        public float time;

        public Move(Vector3 position, float time)
        {
            this.position = position;
            this.time = time;
        }
    }
}
