using System;
using System.Collections.Generic;
using System.Linq;
using Libs;

namespace Network.Events
{
    public class EventManager
    {
        private static EventManager instance = null;
        
        private SortedList<int, SortedList<int, IEvent>> eventListList = new SortedList<int, SortedList<int, IEvent>>();

        private EventManager()
        {
            foreach (EventTimeoutTypeEnum eventTimeoutType in Enum.GetValues(typeof(EventTimeoutTypeEnum)))
            {
                eventListList.Add((int)eventTimeoutType,new SortedList<int,IEvent>());
            }
        }

        public static EventManager GetInstance()
        {
            return instance ?? (instance = new EventManager());
        }
        
        public void addEvent(IEvent iEvent)
        {
            SortedList<int, IEvent> eventList = GetEventSortedList(GetEventTimeoutType(iEvent.GetEventEnum()));
            eventList.Add(iEvent.GetSeqId(),iEvent);
        }
        
        public void clearEventList(EventTimeoutTypeEnum eventTimeoutType, int seq_id)
        {
            SortedList<int, IEvent> eventList = GetEventSortedList(eventTimeoutType);
            int index = eventListList.IndexOfKey((int)eventTimeoutType);
            eventListList[index] = (SortedList<int,IEvent>)eventList.Where((eventt,innerIndex) => innerIndex > seq_id);
        }
        public void clearEventList(EventEnum eventEnum, int seq_id)
        {
            clearEventList(GetEventTimeoutType(eventEnum),seq_id);
        }
        public void clearEventList(IEvent iEvent)
        {
            clearEventList(iEvent.GetEventEnum(),iEvent.GetSeqId());
        }
        
        public SortedList<int,IEvent> GetEventSortedList(EventTimeoutTypeEnum eventTimeoutType)
        {
            int index = eventListList.IndexOfKey((int)eventTimeoutType);
            return eventListList[index];
        }
        public SortedList<int,IEvent> GetEventSortedList(EventEnum eventEnum)
        {
            return GetEventSortedList(GetEventTimeoutType(eventEnum));
        }
        public SortedList<int,IEvent> GetEventSortedList(IEvent iEvent)
        {
            return GetEventSortedList(iEvent.GetEventEnum());
        }
        
        public List<IEvent> getEventList(EventTimeoutTypeEnum eventTimeoutType)
        {
            return (List<IEvent>) GetEventSortedList(eventTimeoutType).Values;
        }
        public List<IEvent> getEventList(EventEnum eventEnum)
        {
            return (List<IEvent>) GetEventSortedList(eventEnum).Values;
        }
        public List<IEvent> getEventList(IEvent iEvent)
        {
            return (List<IEvent>) GetEventSortedList(iEvent).Values;
        }
        
        // Scalable to multiple timeouts
        public static EventTimeoutTypeEnum GetEventTimeoutType(EventEnum eventEnum)
        {
            switch (eventEnum)
            {
                case EventEnum.EventCreation:
                    return EventTimeoutTypeEnum.TimeOut10;
                    break;
                case EventEnum.EventColor:
                case EventEnum.EventNewProjectile:
                default:
                    return EventTimeoutTypeEnum.NoTimeOut;
                    break;
            }
        }

        public IEvent readEvent(BitBuffer bitBuffer)
        {
            //Read id;
            int id = bitBuffer.readInt(1, GetTotalEventEnum());
            IEvent iEvent;
            switch ((EventEnum)id)
            {
                case EventEnum.EventCreation:
                    iEvent = new CreationEvent(bitBuffer);
                    break;
                case EventEnum.EventColor:
                    iEvent = null;
                    break;
                case EventEnum.EventNewProjectile:
                    iEvent = null;
                    break;               
                default:
                    iEvent = null;
                    break;
            }

            return iEvent;
        }
        
        public static int GetTotalEventEnum()
        {
            return Enum.GetValues(typeof(EventEnum)).Length;
        }

    }
    
}