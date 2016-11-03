using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace PSXBuilderNetworking.Messages
{
    public class BuildSessionStartMessage : SessionStartMessage
    {
        public String ProjectPath { get; set; }
        public String SDKPath     { get; set; }
        public String Output      { get; set; }

        protected override void AppendData(ByteArrayWriter writer)
        {
            writer.Append(GetStringSize(ProjectPath));
            writer.Append(ProjectPath);

            writer.Append(GetStringSize(SDKPath));
            writer.Append(SDKPath);

            writer.Append(GetStringSize(Output));
            writer.Append(Output);
            base.AppendData(writer);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(Output)      +
                   sizeof(int) + GetStringSize(ProjectPath) +
                   sizeof(int) + GetStringSize(SDKPath)     +
                   base.GetDataSize();
        }

        protected override void ReadData(ByteArrayReader reader)
        {
            ProjectPath = reader.ReadString(reader.ReadInt());
            SDKPath     = reader.ReadString(reader.ReadInt());
            Output      = reader.ReadString(reader.ReadInt());
            base.ReadData(reader);
        }
    }
}
