using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class PartialMessage : Message
    {
        public Byte[] Data { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(Data);
        }

        protected override int GetDataSize()
        {
            return Data.Length;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Data = arrayReader.ReadAll();
        }
    }
}
