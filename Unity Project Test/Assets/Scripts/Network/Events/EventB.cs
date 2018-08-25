using UnityEngine;

namespace Network.Events
{
    public class EventB : IEvent
    {
        public override byte[] GetByteArray()
        {
            
            buffer.writeFloat(rotation.x,0f,5f,0.1f);
            buffer.writeFloat(rotation.y,0f,5f,0.1f);
            buffer.writeFloat(rotation.z,0f,5f,0.1f);
            buffer.writeFloat(rotation.w,0f,5f,0.1f);
            return buffer.getBuffer();
        }

        private Quaternion rotation;
        
    }
}