using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class FileListMessage : Message
    {
        public List<String> Files { get; set; }

        public FileListMessage()
        {
            Files = new List<String>();
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            foreach (var file in Files)
            {
                arrayWriter.Append(GetStringSize(file));
                arrayWriter.Append(file);
            }
        }

        protected override int GetDataSize()
        {
            int result = 0;
            foreach (var file in Files)
            {
                result += sizeof(int) + GetStringSize(file);
            }
            return result;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            while(!arrayReader.IsEmpty())
            {
                Files.Add(arrayReader.ReadString(arrayReader.ReadInt()));
            }
        }
    }
}
