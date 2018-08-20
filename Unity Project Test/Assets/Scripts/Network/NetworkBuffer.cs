using System.Collections.Generic;
using UnityEngine;

public class NetworkBuffer<T> {

    Queue<NetworkItem<T>> networkQueue;
    private List<NetworkItem<T>> networkItemList = new List<NetworkItem<T>>(5);
    private float lastTime;

    public NetworkBuffer()
    {
        networkQueue = new Queue<NetworkItem<T>>();
        lastTime = 0;
    }

    public void AddItem(T item, float time)
    {
        networkQueue.Enqueue(new NetworkItem<T>(item, time));
    }

    public InterpolatedItem<T> GetNextItem()
    {
        while (networkQueue.Count > 0)
        {
            NetworkItem<T> m = networkQueue.Dequeue();
            if (networkItemList.Count == networkItemList.Capacity)
            {
                networkItemList.RemoveAt(0);
            }
            networkItemList.Add(m);
        }
        if (networkItemList.Count > 3)
        {
            if (lastTime == 0)
            {
                NetworkItem<T> m = networkItemList[0];
                lastTime = m.time;
                return new InterpolatedItem<T>(networkItemList[0].item, networkItemList[1].item, 0);
            }
            else
            {
                float time = lastTime + Time.deltaTime;
                NetworkItem<T> m;
                NetworkItem<T> lastNetworkItem;
                int i = 1;
                do
                {
                    m = networkItemList[i];
                    lastNetworkItem = networkItemList[i - 1];
                    i++;
                } while (m.time < time && i < networkItemList.Count);

                float interpolation = (time - lastNetworkItem.time) / (m.time - lastNetworkItem.time);
                lastTime += Time.deltaTime;
                return new InterpolatedItem<T>(lastNetworkItem.item, m.item, interpolation);
            }
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

    private class NetworkItem<S>
    {
        public S item;
        public float time;

        public NetworkItem(S item, float time)
        {
            this.item = item;
            this.time = time;
        }
    }
}
