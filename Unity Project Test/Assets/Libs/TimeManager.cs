namespace Libs
{
    public class TimeManager
    {
        private float _elapsedTime;
        private readonly float _maxCycleTime;

        public TimeManager(float maxCycleTime)
        {
            _maxCycleTime = maxCycleTime;
        }

        public TimeManager(float maxCycleTime, float elapsedTime)
        {
            _elapsedTime = elapsedTime;
            _maxCycleTime = maxCycleTime;
        }

        public void UpdateTime(float deltaTime)
        {
            _elapsedTime = (_elapsedTime + deltaTime) % _maxCycleTime;
        }

        public float GetCurrentTime()
        {
            return _elapsedTime;
        }

        public void SetElapsedTime(float time)
        {
            _elapsedTime = time;
        }
    }
}