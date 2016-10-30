using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class LogMessage : Message
    {
        public String Text { get; set; }

        public LogMessage()
        {
            Text = "";
        }

        public LogMessage(String text)
        {
            Text = text;
        }

        public LogMessage(String format, params object[] objects)
        {
            Text = String.Format(format, objects);
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(Text);
        }

        protected override int GetDataSize()
        {
            return GetStringSize(Text);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Text = arrayReader.ReadString();
        }
    }
}
