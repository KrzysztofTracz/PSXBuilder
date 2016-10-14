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
            arrayWriter.Append(BitConverter.GetBytes(Parts));
        }

        protected override int GetDataSize()
        {
            return sizeof(int);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Parts = BitConverter.ToInt32(arrayReader.Read(GetDataSize()), 0);
        }
    }
}
