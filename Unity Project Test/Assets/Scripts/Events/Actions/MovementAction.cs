namespace Events.Actions
{
    public class MovementAction : EventAction
    {
        public MovementAction(byte[] payload)
        {
            
        }
        
        public byte[] Serialize()
        {
            throw new System.NotImplementedException();
        }

        public void Execute(GameManager gameManager)
        {
            throw new System.NotImplementedException();
        }

        public EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.NoTimeOut;
        }

        public EventEnum GetEventType()
        {
            return EventEnum.Movement;
        }
        
        public static int GetPayloadBitSize()
        {
            return 0;
        }
    }
    
    public enum MovementEnum
    {
        W = 0, 
        A = 1, 
        S = 2, 
        D = 3, 
        ClickLeft = 4, 
        ClickRight = 5 
    }
}