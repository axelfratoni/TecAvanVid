using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Libs;
using UnityEngine;

namespace Events
{
    public class ReliableEventQueue
    {
        private List<Event> _lastAcked;
        private List<Event> _eventQueue;
        private readonly Timer _timer;

        // Receives an action and a timeout. Every time interval of value timeout the action will be executed.
        public ReliableEventQueue(int timeout, Action<List<Event>> timeoutAction)
        {
            _eventQueue = new List<Event>();
            _lastAcked = new List<Event>();
            _timer = new Timer {Interval = timeout == 0? 1 : timeout};
            _timer.Elapsed += (sender, e) => timeoutAction(_eventQueue);
            _timer.Start();
        }
        
        public void AddEvent(Event iEvent)
        {
            _eventQueue.Add(iEvent);
        }

        // When acking an event, all events of the same kind and same client with lower seq id will be removed.
        // The last acked event will be stored in order to check if it has been already processed.
        public void AckEvent(Event ievent)
        {
            _eventQueue = _eventQueue.Where(ev => !(ev.ClientId == ievent.ClientId &&
                                                    ev.GetEventEnum().Equals(ievent.GetEventEnum()) &&
                                                    UnsignedCircularComparator.compareLong((ulong) ev.SeqId, (ulong) ievent.SeqId, (ulong) Connection.MAX_SEQ_ID) < 1))
                                                    .ToList();
            
            _lastAcked = _lastAcked.Where(ev => !(ev.ClientId == ievent.ClientId &&
                                                  ev.GetEventEnum().Equals(ievent.GetEventEnum()))).ToList();
            _lastAcked.Add(ievent);
        }

        // Checks whether the event has been processed already or not.
        public bool ShouldProcessEvent(Event ievent)
        {
            Event lastAckedEvent = _lastAcked.Find(ev => ev.ClientId == ievent.ClientId &&
                                                         ev.GetEventEnum().Equals(ievent.GetEventEnum()));
            return lastAckedEvent == null || 
                   UnsignedCircularComparator.compareLong((ulong) ievent.SeqId, (ulong) lastAckedEvent.SeqId, (ulong) Connection.MAX_SEQ_ID) <= 1;
        }

        public void Disable()
        {
            _timer.Close();
        }

        ~ReliableEventQueue()
        {
            Disable();
        }
    }
}