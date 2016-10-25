using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class ProcessResultMessage : Message
    {
        public int    ReturnCode { get; set; }
        public String Output     { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(ReturnCode);
            arrayWriter.Append(Output);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(Output);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            ReturnCode = arrayReader.ReadInt();
            Output     = arrayReader.ReadString();
        }
    }
}
