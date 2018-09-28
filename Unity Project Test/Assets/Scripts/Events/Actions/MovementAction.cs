using Libs;

namespace Events.Actions
{
    public class MovementAction : EventAction
    {
        public MovementAction(BitBuffer payload)
        {
            
        }

        public override void Serialize(BitBuffer buffer)
        {
            throw new System.NotImplementedException();
        }

        public override void Execute(GameManager gameManager)
        {
            throw new System.NotImplementedException();
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.NoTimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Movement;
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