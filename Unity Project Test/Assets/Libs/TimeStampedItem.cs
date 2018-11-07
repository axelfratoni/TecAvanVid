namespace Libs
{
    public class TimeStampedItem<TR>
    {
        public float Time { get; private set; }
        public TR Item { get; private set; }

        public TimeStampedItem(float time, TR item)
        {
            Time = time;
            Item = item;
        }
    }
}