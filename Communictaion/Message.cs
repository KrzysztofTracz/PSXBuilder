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
                if(_library == null)
                {
                    _library = new MessageLibrary();
                }
                return _library;
            }
        }

        public static int GetHeaderSize()
        {
            return sizeof(Byte) + sizeof(Int16);
        }

        public Byte ID { get; protected set; }

        public Int16 Size
        {
            get
            {
                return (Int16)(GetHeaderSize() + GetDataSize());
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

            return arrayWriter.GetArray();
        }

        public virtual void FromByteArray(Byte[] array)
        {
            var arrayReader = new ByteArrayReader(array);
            ReadData(arrayReader);
        }

        protected abstract Int16 GetDataSize();
        protected abstract void  AppendData(ByteArrayWriter arrayWriter);
        protected abstract void  ReadData  (ByteArrayReader arrayReader);

        private static MessageLibrary _library = null;
    }
}
