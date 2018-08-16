using System;
using System.IO;
using UnityEngine.Analytics;

namespace Libs
{
    public class BitBuffer
    {
        private static bool READ = true;
        private static bool WRITE = false;
        private bool mode;
        private long bits;
        private int currentBitCount;
        private MemoryStream buffer;

        public BitBuffer(int buffer_length)
        {
            bits = 0;
            currentBitCount = 0;
            buffer = new MemoryStream(capacity:buffer_length);
            mode = WRITE;
        }
        
        public BitBuffer(byte[] buffer)
        {
            bits = BitConverter.ToInt64(buffer, 0);
            currentBitCount = 0;
            this.buffer = new MemoryStream(buffer);
            mode = READ;
        }

        public void writeBit(bool toWriteBool){
            long longValue = toWriteBool ? 1L : 0L;
            bits |= longValue << currentBitCount;
            currentBitCount++;
            writeBuffer();
        }

        public bool readBit(){
            bool value = (bits & (1 << currentBitCount)) > 0;
            currentBitCount++;
            //writeBuffer();
            return value;
        }

        public void writeBits(long value, int bitCount){
            for (int i = 0; i < bitCount; i++)
            {
                writeBit((value & (1 << i)) > 0);
            }
        }

        public long readBits(int bitCount)
        {
            long ans = 0;
            for (int i = 0; i < bitCount; i++)
            {
                ans |= readBit() ? 1L : 0L << i;
            }

            return ans;
        }

        public void writeInt(int value, int min, int max){
            writeBits(value-min,GetBitsRequired(max-min));
        }

        public int readInt(int min, int max){
            return (int) (min + readBits(GetBitsRequired(max-min)));
        }

        public void writeFloat(float value, float min, float max, float step){
            writeInt((int)Math.Round((value-min)/step),(int)Math.Round((min)/step),(int)Math.Round((max)/step));
        } 

        public float readFloat(float min, float max, float step){
            return min + step * readInt((int) Math.Round((min) / step), (int) Math.Round((max) / step));
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
        
        private void fillBuffer(long value, int bitCount){
            //TODO
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

        public void flush(){
            writeBits(0,32 - currentBitCount);
            writeBuffer();
        }

        public byte[] getBuffer()
        {
            byte[] ans = buffer.GetBuffer();
            buffer = new MemoryStream(capacity:buffer.Capacity);
            return ans;
        }
    }
}