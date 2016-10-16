using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class PartialMessageStart : Message
    {
        public int Parts     { get; set; } 
        public int TotalSize { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(Parts);
            arrayWriter.Append(TotalSize);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) * 2;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Parts     = arrayReader.ReadInt();
            TotalSize = arrayReader.ReadInt();
        }
    }
}
