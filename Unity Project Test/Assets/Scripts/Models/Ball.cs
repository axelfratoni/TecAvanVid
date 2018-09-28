using UnityEngine;

namespace Network
{
    public class Ball
    {
        private readonly int _objectId;
        private readonly int _clientId;
        private Vector3 _position;

        public Ball(int clientId, int objectId, Vector3 position)
        {
            _objectId = objectId;
            _clientId = clientId;
            _position = position;
        }

        public int ObjectId
        {
            get { return _objectId; }
        }

        public int ClientId
        {
            get { return _clientId; }
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }
    }
}