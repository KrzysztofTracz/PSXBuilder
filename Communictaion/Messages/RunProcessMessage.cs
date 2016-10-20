using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class RunProcessMessage : Message
    {
        public String ExeName { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(ExeName);
        }

        protected override int GetDataSize()
        {
            return GetStringSize(ExeName);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            ExeName = arrayReader.ReadString();
        }
    }
}
