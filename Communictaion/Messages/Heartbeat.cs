using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class Heartbeat : Message
    {
        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            return;
        }

        protected override int GetDataSize()
        {
            return 0;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            return;
        }
    }
}
