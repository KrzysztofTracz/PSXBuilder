using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class CheckFileMessage : Message
    {
        public String Path     { get; set; }
        public long   FileSize { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(FileSize);
            arrayWriter.Append(Path);
        }

        protected override int GetDataSize()
        {
            return sizeof(long) + GetStringSize(Path);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            FileSize = arrayReader.ReadLong();
            Path     = arrayReader.ReadString();
        }
    }
}
