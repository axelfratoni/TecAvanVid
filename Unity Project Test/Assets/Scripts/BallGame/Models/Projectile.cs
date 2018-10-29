using UnityEngine;

namespace Network
{
    public class Projectile
    {
        public readonly int objectId;
        public readonly int clientId;
        public Vector3 position;
        public bool isControlled;
        
        public Projectile(int clientId, int objectId, Vector3 position, bool isControlled)
        {
            this.objectId = objectId;
            this.clientId = clientId;
            this.position = position;
            this.isControlled = isControlled;
        }
    }
}