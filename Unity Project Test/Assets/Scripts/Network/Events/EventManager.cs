using System.Collections.Generic;
using System.Linq;
using UnityEditorInternal;

namespace Network.Events
{
    public class EventManager
    {
        private SortedList<int,EventInterface> withTimeout = new SortedList<int,EventInterface>();
        private SortedList<int,EventInterface> withoutTimeout = new SortedList<int,EventInterface>();

        public void addTimeoutEvent(EventInterface eventt)
        {
            withTimeout.Add(eventt.getSeqId(),eventt);
        }
        public void addNoTimeoutEvent(EventInterface eventt)
        {
            withoutTimeout.Add(eventt.getSeqId(),eventt);
        }
        public void clearTimeoutEvent(int seq_id)
        {
            withTimeout = (SortedList<int,EventInterface>)withTimeout.Where((eventt,index) => index > seq_id);
        }
        public void clearNoTimeoutEvent(int seq_id)
        {
            withoutTimeout = (SortedList<int,EventInterface>)withoutTimeout.Where((eventt,index) => index > seq_id);
        }

        public List<EventInterface> getTimeoutEvents()
        {
            return (List<EventInterface>)withTimeout.Values;
        }
        public List<EventInterface> getNoTimeoutEvents()
        {
            return (List<EventInterface>)withoutTimeout.Values;
        }

        private static EventManager instance = null;
        
        private EventManager(){}

        public static EventManager getInstance()
        {
            return instance ?? (instance = new EventManager());
        }
    }
}