using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Libs;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Events
{
    public class NetworkBuffer<T>
    {
        private readonly LinkedList<TimeStampedItem<T>> _itemList = new LinkedList<TimeStampedItem<T>>();
        private float _elapsedTime;
        
        public void Reset()
        {
            _itemList.Clear();
        }
        
        public void AddItem(T item, float time)
        {
            bool shouldAdd = _itemList.Count == 0 || _itemList.Last.Value.Time < time;
            if (shouldAdd)
            {
                TimeStampedItem<T> nextItem = new TimeStampedItem<T>(time, item);
                _itemList.AddLast(nextItem);
            }
        }
            
        public delegate T Interpolate(T first, T second, float percentage);

        public bool GetNextItem(Interpolate interpolationFunction, float deltaTime, out T interpolatedItem)
        {
            interpolatedItem = default(T);
            
            if (_itemList.First != null && _itemList.First.Next != null)
            {
                _elapsedTime += deltaTime;
                float timeWindow = _itemList.First.Next.Value.Time - _itemList.First.Value.Time;
                while (timeWindow < _elapsedTime && _itemList.Count > 2)
                {
                    _itemList.RemoveFirst();
                    _elapsedTime -= timeWindow;
                    timeWindow = _itemList.First.Next.Value.Time - _itemList.First.Value.Time;
                }
                
                float interpolation = _elapsedTime / timeWindow;
                
                interpolatedItem = interpolationFunction(_itemList.First.Value.Item, _itemList.First.Next.Value.Item, interpolation);

                return true;
            }

            return false;
        }
        
        private class TimeStampedItem<TR>
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
}