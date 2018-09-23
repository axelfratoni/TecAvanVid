namespace Events.Actions
{
    public class CreationAction : EventAction
    {
        public CreationAction(byte[] payload)
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
            return EventTimeoutTypeEnum.TimeOut;
        }

        public EventEnum GetEventType()
        {
            return EventEnum.Creation;
        }
        
        public static int GetPayloadBitSize()
        {
            return 0;
        }
    }
}