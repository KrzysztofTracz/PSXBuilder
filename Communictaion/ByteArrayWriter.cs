﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework
{
    public class ByteArrayWriter
    {
        public int Size     { get; protected set; }
        public int Capacity { get; protected set; }
        public int Cursor   { get; protected set; }

        public ByteArrayWriter(int capacity)
        {
            buffer = new Byte[capacity];

            Capacity = capacity;
            Size     = 0;
            Cursor   = 0;
        }

        public void Append(Byte value)
        {
            if (Size < Capacity)
            {
                buffer[Cursor] = value;
                Cursor++;
                Size++;
            }
        }

        public void Append(Byte[] array)
        {
            int index = 0;
            while(Size < Capacity && index < array.Length)
            {
                buffer[Cursor] = array[index];
                Cursor++;
                Size++;
                index++;
            }
        }

        public void Append(bool value)
        {
            Append(BitConverter.GetBytes(value));
        }

        public void Append(int value)
        {
            Append(BitConverter.GetBytes(value));
        }

        public void Append(long value)
        {
            Append(BitConverter.GetBytes(value));
        }

        public void Append(String value)
        {
            foreach(var c in value)
            {
                Append(BitConverter.GetBytes(c));
            }                       
        }

        public Byte[] GetArray()
        {
            return buffer;
        }

        protected Byte[] buffer = null;
    }
}
