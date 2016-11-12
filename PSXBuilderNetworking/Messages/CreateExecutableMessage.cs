using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;
using CommunicationFramework.Messages;

namespace PSXBuilderNetworking.Messages
{
    public class CreateExecutableMessage : Message
    {
        public String VideoFormat { get; set; }

        public CreateExecutableMessage()
        {
            VideoFormat = "";
        }

        public CreateExecutableMessage(String videoFormat)
        {
            VideoFormat = videoFormat;
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(VideoFormat);
        }

        protected override int GetDataSize()
        {
            return GetStringSize(VideoFormat);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            VideoFormat = arrayReader.ReadString();
        }
    }
}
