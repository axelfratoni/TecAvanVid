using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkBuffer2<T>
{
    private SortedList positionList;
    private float time;

    public NetworkBuffer2()
    {
        positionList = new SortedList();
        time = 0;
    }

    public void AddItem(T item, float time)
    {
        if (positionList.Count == 0 || time > (float)positionList.GetKey(positionList.Count - 1))
        {
            positionList.Add(time, item);
        }
    }

    public InterpolatedItem<T> GetNextItem()
    {
        if (positionList.Count > 2)
        {
            if (time == 0)
            {
                time = (float)positionList.GetKey(0);
            }
            else
            {
                time += Time.deltaTime;
            }
            if (time > (float)positionList.GetKey(1))
            {
                positionList.RemoveAt(0);
            }

            float nextPositionTime = (float)positionList.GetKey(1);
            float previousPositionTime = (float)positionList.GetKey(0);
            float interpolation = ((time - previousPositionTime) / (nextPositionTime - previousPositionTime));
            return new InterpolatedItem<T>((T)positionList.GetByIndex(0), (T)positionList.GetByIndex(1), interpolation);
        }
        return null;
    }

    public class InterpolatedItem<R>
    {
        public R previous;
        public R next;
        public float interpolation;

        public InterpolatedItem(R previous, R next, float interpolation)
        {
            this.previous = previous;
            this.next = next;
            this.interpolation = interpolation;
        }
    }
}
