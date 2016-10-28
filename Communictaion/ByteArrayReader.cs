using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework
{
    public class ByteArrayReader
    {
        public int Cursor { get; protected set; }

        public int BytesLeft
        {
            get { return buffer.Length - Cursor; }
        }

        public ByteArrayReader(Byte[] array)
        {
            buffer = array;
            Cursor = 0;
        }

        public bool IsEmpty()
        {
            return BytesLeft == 0;
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

        public bool ReadBool()
        {
            return BitConverter.ToBoolean(Read(sizeof(bool)), 0);
        }

        public int ReadInt()
        {
            return BitConverter.ToInt32(Read(sizeof(int)), 0);
        }

        public long ReadLong()
        {
            return BitConverter.ToInt64(Read(sizeof(long)), 0);
        }

        public String ReadString(int size)
        {
            StringBuilder sb = new StringBuilder();
            size = size / sizeof(char);
            for(int i=0;i<size;i++)
            {
                sb.Append(BitConverter.ToChar(Read(sizeof(char)),0));
            }
            return sb.ToString();
        }

        public String ReadString()
        {
            return ReadString(BytesLeft);
        }

        public Byte[] ReadAll()
        {
            return Read(BytesLeft);
        }

        protected Byte[] buffer = null;
    }
}
