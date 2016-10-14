using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class PartialMessageReceived : Message
    {
        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            
        }

        protected override int GetDataSize()
        {
            return 0;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            
        }
    }
}
