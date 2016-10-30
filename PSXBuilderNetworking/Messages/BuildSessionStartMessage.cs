using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace PSXBuilderNetworking.Messages
{
    public class BuildSessionStartMessage : SessionStartMessage
    {
        public String Output { get; set; }

        protected override void AppendData(ByteArrayWriter writer)
        {
            writer.Append(GetStringSize(Output));
            writer.Append(Output);
            base.AppendData(writer);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(Output) +
                   base.GetDataSize();
        }

        protected override void ReadData(ByteArrayReader reader)
        {
            Output = reader.ReadString(reader.ReadInt());
            base.ReadData(reader);
        }
    }
}
