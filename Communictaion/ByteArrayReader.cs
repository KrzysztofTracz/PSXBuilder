using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework
{
    public class ByteArrayReader
    {
        public int Cursor { get; protected set; }

        public ByteArrayReader(Byte[] array)
        {
            buffer = array;
            Cursor = 0;
        }

        public Byte Read()
        {
            Byte result = 0;
            if(Cursor < buffer.Length)
            {
                result = buffer[Cursor];
                Cursor++;
            }
            return result;
        }

        public Byte[] Read(int size)
        {
            var result = new Byte[size];
            int index  = 0;

            while(Cursor < buffer.Length && index < size)
            {
                result[index] = buffer[Cursor];
                Cursor++;
                index++;
            }

            return result;
        }

        public Byte[] ReadAll()
        {
            return Read(buffer.Length - Cursor);
        }

        protected Byte[] buffer = null;
    }
}
