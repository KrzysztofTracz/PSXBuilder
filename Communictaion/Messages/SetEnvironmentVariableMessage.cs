using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class SetEnvironmentVariableMessage : Message
    {
        public String Name  { get; set; }
        public String Value { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(GetStringSize(Name));
            arrayWriter.Append(Name);

            arrayWriter.Append(GetStringSize(Value));
            arrayWriter.Append(Value);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(Name) +
                   sizeof(int) + GetStringSize(Value);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Name  = arrayReader.ReadString(arrayReader.ReadInt());
            Value = arrayReader.ReadString(arrayReader.ReadInt());
        }
    }
}
