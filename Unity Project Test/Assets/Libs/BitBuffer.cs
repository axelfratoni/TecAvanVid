using System;
using System.IO;

namespace Libs
{
    public class BitBuffer
    {
        private long bits;
        private int currentBitCount;
        private MemoryStream buffer;

        public BitBuffer(long bits, int currentBitCount, int buffer_length)
        {
            this.bits = bits;
            this.currentBitCount = currentBitCount;
            buffer = new MemoryStream(capacity:1024);
        }

        public void writeBit(bool toWriteBool){
            long longValue = toWriteBool ? 1L : 0L;
            bits |= longValue << currentBitCount;
            currentBitCount++;
            writeBuffer();
        }

        public void writeBits(long value, int bitCount){
            for (int i = 0; i < bitCount; i++)
            {
                writeBit((value & (1 << bitCount)) > 0);
            }
        }

        public void writeInt(int value, int min, int max){
            writeBits(value-min,GetBitsRequired(max-min));
        }

        public void writeFloat(float value, float min, float max, float step){
            writeInt((int)Math.Round((value-min)/step),(int)Math.Round((min)/step),(int)Math.Round((max)/step));
        } 
        
        private static int GetBitsRequired(long value) {
            int bitsRequired = 0;
            while (value > 0) {
                bitsRequired++;
                value >>= 1;
            }
            return bitsRequired;
        }

        private void writeBuffer(){
            if (currentBitCount >= 32) {
                if (buffer.Position + 4 > buffer.Capacity) {
                    throw new InvalidOperationException("write buffer overflow");
                }
                int word = (int) bits;
                byte a = (byte) (word);
                byte b = (byte) (word >> 8);
                byte c = (byte) (word >> 16);
                byte d = (byte) (word >> 24);
                buffer.WriteByte(d);
                buffer.WriteByte(c);
                buffer.WriteByte(b);
                buffer.WriteByte(a);
                bits >>= 32;
                currentBitCount -= 32;
            }
        }

        public void flash(){
            writeBits(0,32 - currentBitCount);
            writeBuffer();
        }

        public MemoryStream getBuffer(){
            return buffer;
        }
    }
}