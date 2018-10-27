namespace Events
{
    public class LatencyEvent
    {
        private Event _ievent;
        private double _latency;

        public LatencyEvent(Event ievent, double latency)
        {
            _ievent = ievent;
            _latency = latency;
        }

        public double Latency
        {
            get { return _latency; }
            set { _latency = value; }
        }
        
        public Event IEvent
        {
            get { return _ievent; }
        }
    }
}