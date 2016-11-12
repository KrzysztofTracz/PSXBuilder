using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace PSXBuilderNetworking.Messages
{
    public class SessionStartMessage : Message
    {
        public String User          { get; set; }
        public String Project       { get; set; }
        public String Configuration { get; set; }

        public SessionStartMessage()
        {
            User          = "";
            Project       = "";
            Configuration = "";
        }

        public SessionStartMessage(String user, String project, String configuration)
        {
            User          = user;
            Project       = project;
            Configuration = configuration;
        }

        protected override void AppendData(ByteArrayWriter writer)
        {
            writer.Append(GetStringSize(User));
            writer.Append(User);

            writer.Append(GetStringSize(Project));
            writer.Append(Project);

            writer.Append(GetStringSize(Configuration));
            writer.Append(Configuration);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(User)        +
                   sizeof(int) + GetStringSize(Project)     +
                   sizeof(int) + GetStringSize(Configuration);
        }

        protected override void ReadData(ByteArrayReader reader)
        {
            User          = reader.ReadString(reader.ReadInt());
            Project       = reader.ReadString(reader.ReadInt());
            Configuration = reader.ReadString(reader.ReadInt());
        }
    }
}
