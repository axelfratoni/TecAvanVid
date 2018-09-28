using System;
using Libs;
using UnityEngine;

namespace Events.Actions
{
    public class ColorAction : EventAction
    {
        private int _r;
        private int _g;
        private int _b;
        
        public ColorAction(int r, int g, int b)
        {
            _r = r;
            _g = g;
            _b = b;
        }
        
        public ColorAction(BitBuffer payload)
        {
            _r = payload.readInt(0, 300);
            _g = payload.readInt(0, 300);
            _b = payload.readInt(0, 300);
        }
        
        public override void Serialize(BitBuffer buffer)
        {
            buffer.writeInt(_r, 0, 300);
            buffer.writeInt(_g, 0, 300);
            buffer.writeInt(_b, 0, 300);
        }

        public override void Execute(WorldManager worldManager)
        {
            worldManager.ProcessColorAction(_r, _g, _b);
        }

        public override EventTimeoutTypeEnum GetTimeoutType()
        {
            return EventTimeoutTypeEnum.TimeOut;
        }

        public override EventEnum GetEventType()
        {
            return EventEnum.Color;
        }
    }
}