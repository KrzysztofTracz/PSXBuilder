using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class RunProcessMessage : Message
    {
        public String Process     { get; set; }
        public String Arguments   { get; set; }
        public bool   WaitForExit { get; set; }

        public RunProcessMessage()
        {
            Process     = "";
            Arguments   = "";
            WaitForExit = false;
        }

        public RunProcessMessage(String process, params String[] arguments)
        {
            Process     = process;
            Arguments   = ApplicationFramework.Utils.ConcatArguments(" ", arguments);
            WaitForExit = false;
        }

        public RunProcessMessage(String process, bool waitForExit = false, params String[] arguments)
        {
            Process     = process;
            Arguments   = ApplicationFramework.Utils.ConcatArguments(" ", arguments);
            WaitForExit = waitForExit;
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(WaitForExit);

            arrayWriter.Append(GetStringSize(Process));
            arrayWriter.Append(Process);

            arrayWriter.Append(GetStringSize(Arguments));
            arrayWriter.Append(Arguments);
        }

        protected override int GetDataSize()
        {
            return sizeof(bool) +
                   sizeof(int)  + GetStringSize(Process) +
                   sizeof(int)  + GetStringSize(Arguments);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            WaitForExit = arrayReader.ReadBool();
            Process     = arrayReader.ReadString(arrayReader.ReadInt());
            Arguments   = arrayReader.ReadString(arrayReader.ReadInt());
        }
    }
}
