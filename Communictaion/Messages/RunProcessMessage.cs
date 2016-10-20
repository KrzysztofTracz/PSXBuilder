using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class RunProcessMessage : Message
    {
        public String Process   { get; set; }
        public String Arguments { get; set; }

        public RunProcessMessage()
        {
            Process   = "";
            Arguments = "";
        }

        public RunProcessMessage(String process, params String[] arguments)
        {
            Process   = process;
            Arguments = ApplicationFramework.Utils.ConcatArguments(" ", arguments);
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(GetStringSize(Process));
            arrayWriter.Append(Process);

            arrayWriter.Append(GetStringSize(Arguments));
            arrayWriter.Append(Arguments);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(Process) +
                   sizeof(int) + GetStringSize(Arguments);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Process   = arrayReader.ReadString(arrayReader.ReadInt());
            Arguments = arrayReader.ReadString(arrayReader.ReadInt());
        }
    }
}
