using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace PSXBuilderNetworking.Messages
{
    public class BuildSessionStartMessage : Message
    {
        public String User    { get; set; }
        public String Project { get; set; }

        public BuildSessionStartMessage()
        {
            User    = "";
            Project = "";
        }

        public BuildSessionStartMessage(String user, String project)
        {
            User    = user;
            Project = project;
        }

        protected override void AppendData(ByteArrayWriter writer)
        {
            writer.Append(GetStringSize(User));
            writer.Append(User);

            writer.Append(GetStringSize(Project));
            writer.Append(Project);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(User) +
                   sizeof(int) + GetStringSize(Project);
        }

        protected override void ReadData(ByteArrayReader reader)
        {
            User    = reader.ReadString(reader.ReadInt());
            Project = reader.ReadString(reader.ReadInt());
        }
    }
}
