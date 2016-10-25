using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class CheckFileResultMessage : Message
    {
        public bool Result { get; set; }

        public CheckFileResultMessage()
            : base()
        {
        }

        public CheckFileResultMessage(bool result)
        {
            Result = result;
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(Result);
        }

        protected override int GetDataSize()
        {
            return sizeof(bool);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Result = arrayReader.ReadBool();
        }
    }
}
