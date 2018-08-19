using Libs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkCube : MonoBehaviour {

    public int listeningPort = 11000;

    private UDPChannel udpReceiver;
    private SortedList positionList;
    private float time;
    Queue<Move> qqueue = new Queue<Move>();
    private List<Move> moveList = new List<Move>(5);
    private float lastTime = 0;
    private static float windowTime = 0.1f;

    void Start()
    {
        udpReceiver = new UDPChannel(listeningPort, ListenAction);
        positionList = new SortedList();
        time = 0;
    }

    void ListenAction(Byte[] receivedBytes)
    {
        BitBuffer bitBuffer = new BitBuffer(receivedBytes);
        float x = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        float y = bitBuffer.readFloat(-1.0f, 15.0f, 0.1f);
        float z = bitBuffer.readFloat(-31.0f, 31.0f, 0.1f);
        float qx = bitBuffer.readFloat(-1.0f, 1.0f, 0.01f);
        float qy = bitBuffer.readFloat(-1.0f, 1.0f, 0.01f);
        float qz = bitBuffer.readFloat(-1.0f, 1.0f, 0.01f);
        float qw = bitBuffer.readFloat(-1.0f, 1.0f, 0.01f);
        float time = bitBuffer.readFloat(0.0f, 3600.0f, 0.01f);
        //Debug.Log("Ser Pos: " + x + " " + y + " " + z + " ");
        //Debug.Log("Ser Quaternion: " + qx + " " + qy + " " + qz + " " + qw + " ");
        qqueue.Enqueue(new Move(new Vector3(x, y, z), new Quaternion(qx, qy, qz, qw), time));
    }

    // Update is called once per frame
    void Update () {
        //Clean Queue
        while (qqueue.Count > 0)
        {
            Move m = qqueue.Dequeue();
            if (moveList.Count == moveList.Capacity)
            {
                moveList.RemoveAt(0);
            }
            moveList.Add(m);
        }
        //Ignore while there is no moves
        if (moveList.Count > 3)
        {
            //Init
            if (lastTime == 0)
            {
                Move m = moveList[0];
                transform.position = m.getPosition();
                transform.rotation = m.getRotation();
                lastTime = m.getTime();
                //Debug.Log(lastTime);
            }
            else
            {
                //Debug.Log(Time.deltaTime);
                float time = lastTime + Time.deltaTime;
                Move m;
                Move lastMove;
                int i = 1;
                //Get next saved move
                do
                {
                    m = moveList[i];
                    lastMove = moveList[i - 1];
                    i++;
                    //Debug.Log("get"+ m.getTime() + " time" + time + "   " + moveList.Count);
                } while (m.getTime() < time && i < moveList.Count);

                Vector3 temp = (m.getPosition() - lastMove.getPosition()) * (time - lastMove.getTime()) / (m.getTime() - lastMove.getTime());
                Debug.Log("TEM" + temp + "TEM" + m + "Last" + lastMove.getPosition());
                Debug.Log("Last" + Time.deltaTime);
                transform.position = lastMove.getPosition() + temp;
                lastTime += Time.deltaTime;
            }
        }
    }

    private void OnDisable()
    {
        udpReceiver.Disable();
    }

    private class Move
    {
        private Vector3 position;
        private Quaternion rotation;
        private float time;

        public Move(Vector3 position, Quaternion rotation, float time)
        {
            this.position = position;
            this.rotation = rotation;
            this.time = time;
        }

        public Vector3 getPosition()
        {
            return position;
        }

        public Quaternion getRotation()
        {
            return rotation;
        }

        public float getTime()
        {
            return time;
        }
    }
}
