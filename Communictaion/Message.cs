using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework
{
    public abstract class Message
    {
        public static MessageLibrary Library
        {
            get
            {
                return MessageLibrary.Instance;
            }
        }

        public static int GetHeaderSize()
        {
            return sizeof(Byte) + sizeof(int);
        }

        public static int SizeLimit
        {
            get { return 1024; }
        }

        public Byte ID { get; protected set; }

        public int Size
        {
            get
            {
                return GetHeaderSize() + GetDataSize();
            }
        }

        public Message()
        {
            if(Library.IsInitialized)
            {
                ID = Library.GetID(GetType());
            }
        }

        public void InitializeID(Byte id)
        {
            ID = id;
        }

        public virtual Byte[] ToByteArray()
        {
            var arrayWriter = new ByteArrayWriter(Size);

            arrayWriter.Append(ID);
            arrayWriter.Append(BitConverter.GetBytes(Size));

            AppendData(arrayWriter);

            return arrayWriter.GetArray();
        }

        public virtual void FromByteArray(Byte[] array)
        {
            var arrayReader = new ByteArrayReader(array);
            ReadData(arrayReader);
        }

        public String GetName()
        {
            return GetType().Name;
        }

        public static int GetStringSize(String value)
        {
            return sizeof(char) * value.Length;
        }

        protected abstract int   GetDataSize();
        protected abstract void  AppendData(ByteArrayWriter arrayWriter);
        protected abstract void  ReadData  (ByteArrayReader arrayReader);

        private static MessageLibrary _library = null;
    }
}
