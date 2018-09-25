using UnityEngine;

namespace Events.Actions
{
    public class ColorAction : EventAction
    {
        public ColorAction()
        {
        }
        
        public ColorAction(byte[] payload)
        {
        }
        
        public byte[] Serialize()
        {
            return new byte[0];
        }

        public void Execute(GameManager gameManager)
        {
            gameManager.ProcessColorAction();
        }

        public EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.TimeOut;
        }

        public EventEnum GetEventType()
        {
            return EventEnum.Color;
        }

        public static int GetPayloadBitSize()
        {
            return 0;
        }
    }
}