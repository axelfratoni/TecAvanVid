using UnityEngine;

namespace Network
{
    public class BallEric
    {
        private int local_id;
        private Vector3 position;

        protected BallEric(Vector3 position, int localId)
        {
            this.local_id = localId;
            this.position = position;
        }

        public int LocalId
        {
            get { return local_id; }
        }
        
        public Vector3 Position
        {
            set { position = value; }
            get { return position; }
        }
    }
}