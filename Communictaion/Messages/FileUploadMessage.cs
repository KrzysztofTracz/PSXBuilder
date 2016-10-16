using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class FileUploadMessage : Message
    {
        public String FileName { get; set; }
        public Byte[] File     { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(GetStringSize(FileName));
            arrayWriter.Append(FileName);
            arrayWriter.Append(File.Length);
            arrayWriter.Append(File);
        }

        protected override int GetDataSize()
        {
            return sizeof(int) + GetStringSize(FileName) +
                   sizeof(int) + File.Length;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            FileName = arrayReader.ReadString(arrayReader.ReadInt());
            File     = arrayReader.Read      (arrayReader.ReadInt());
        }
    }
}
