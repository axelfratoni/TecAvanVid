using System;
using Libs;
using UnityEngine;

namespace Network.Events
{
    public class ColorEvent: IEvent
    {
        private static int rBitsRequired;
        private static int gBitsRequired;
        private static int bBitsRequired;
        private static int aBitsRequired;
        private static int seqBitsRequired;
        private static int bitsRequired;

        private float _red;
        private float _green;
        private float _blue;
        private float _alpha;

        //This will run only once
        static ColorEvent()
        {
            rBitsRequired = BitBuffer.GetBitsRequiredForFloat(0.0f,1.0f,0.001f);
            gBitsRequired = BitBuffer.GetBitsRequiredForFloat(0.0f,1.0f,0.001f);
            bBitsRequired = BitBuffer.GetBitsRequiredForFloat(0.0f,1.0f,0.001f);
            aBitsRequired = BitBuffer.GetBitsRequiredForFloat(0.0f,1.0f,0.001f);
            seqBitsRequired = BitBuffer.GetBitsRequired(Int32.MaxValue);
            bitsRequired = rBitsRequired + gBitsRequired + bBitsRequired + aBitsRequired + seqBitsRequired;
        }

        public static int GetBitsRequired()
        {
            return bitsRequired;
        }

        public ColorEvent(int red, int green, int blue, int alpha, int seq_id)
        {
            
            eventEnum = EventEnum.EventColor;
            buffer = new BitBuffer(bitsRequired);
            _red = red;
            _blue = blue;
            _green = green;
            _alpha = alpha;
            this.seq_id = seq_id;
        }

        public ColorEvent(BitBuffer bitBuffer)
        {
            float red = bitBuffer.readFloat(0.0f,1.0f,0.001f);
            float blue = bitBuffer.readFloat(0.0f,1.0f,0.001f);
            float green = bitBuffer.readFloat(0.0f,1.0f,0.001f);
            float alpha = bitBuffer.readFloat(0.0f,1.0f,0.001f);
            _red = red;
            _blue = blue;
            _green = green;
            _alpha = alpha;
            seq_id = bitBuffer.readInt(0, Int32.MaxValue);
        }

        public override void Process(GameObject gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.material.color = new Color(_red,_green,_blue,_alpha);
        }

        public override byte[] GetByteArray()
        {

            buffer.writeInt((int) eventEnum, 1, EventManager.GetTotalEventEnum());
            buffer.writeFloat(_red, 0.0f, 1.0f, 0.001f);
            buffer.writeFloat(_blue, 0.0f, 1.0f, 0.001f);
            buffer.writeFloat(_green, 0.0f, 1.0f, 0.001f);
            buffer.writeFloat(_alpha, 0.0f, 1.0f, 0.001f);
            buffer.writeInt(seq_id, 0, Int32.MaxValue);
            return buffer.getBuffer();
        }
    }
}