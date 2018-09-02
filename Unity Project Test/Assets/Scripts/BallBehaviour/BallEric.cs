using UnityEngine;

namespace Network
{
    public abstract class BallEric : MonoBehaviour
    {
        private int local_id;

        protected BallEric()
        {
            local_id = NetworkManager.GetNewLocalId();
        }

        public int LocalId
        {
            get { return local_id; }
        }
    }
}