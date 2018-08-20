using Libs;
using System;
using UnityEngine;

public class NetworkCube : MonoBehaviour {

    public int listeningPort = 11000;

    private UDPChannel udpReceiver;
    private NetworkBuffer<Move> networkBuffer;

    void Start()
    {
        udpReceiver = new UDPChannel(listeningPort, ListenAction);
        networkBuffer = new NetworkBuffer<Move>();
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
        networkBuffer.AddItem(new Move(new Vector3(x, y, z), new Quaternion(qx, qy, qz, qw), time), time);
    }

    void Update () {
        NetworkBuffer<Move>.InterpolatedItem<Move> interpolatedItem = networkBuffer.GetNextItem();
        if(interpolatedItem != null)
        {
            Vector3 temp = (interpolatedItem.next.getPosition() - interpolatedItem.previous.getPosition()) * interpolatedItem.interpolation;
            transform.position = interpolatedItem.previous.getPosition() + temp;
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
