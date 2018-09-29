using System;
using Libs;

namespace Events.Actions
{
    public abstract class EventAction
    {
        public abstract void Serialize(BitBuffer buffer);
                
        public abstract void Execute(WorldManager worldManager, int clientId);

        public abstract EventTimeoutTypeEnum GetTimeoutType();

        public abstract EventEnum GetEventType();

        public static EventAction ExtractAction(BitBuffer buffer, EventEnum eventType)
        {
            EventAction eventAction;
            switch (eventType)
            {
                case EventEnum.Connection:
                    eventAction = new ConnectionAction(buffer);
                    break;
                case EventEnum.Snapshot:
                    eventAction = new SnapshotAction(buffer);
                    break;
                case EventEnum.CreationRequest:
                    eventAction = new CreationRequestAction(buffer);
                    break;
                case EventEnum.Creation:
                    eventAction = new CreationAction(buffer);
                    break;
                case EventEnum.Movement:
                    eventAction = new MovementAction(buffer);
                    break;
                case EventEnum.Color:
                    eventAction = new ColorAction(buffer);
                    break;
                default:
                    throw new Exception("No implementation for event type " + eventType);
            }

            return eventAction;
        }
    }
}