using UnityEngine;

namespace Events.Actions
{
    public interface EventAction
    {
        byte[] Serialize();
        
        void Execute(GameManager gameManager);

        EventTimeoutTypeEnum GetTimeoutType();

        EventEnum GetEventType();
    }
}