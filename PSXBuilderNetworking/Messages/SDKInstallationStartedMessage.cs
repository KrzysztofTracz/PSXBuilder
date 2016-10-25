using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace PSXBuilderNetworking.Messages
{
    public class SDKInstallationStartedMessage : CommunicationFramework.Message
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
