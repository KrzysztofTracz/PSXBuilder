using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class CheckFilesMessage : Message
    {
        public Dictionary<String, long> Files { get; set; }

        public CheckFilesMessage()
        {
            Files = new Dictionary<String, long>();
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            var keys = Files.Keys;
            foreach (var key in keys)
            {
                arrayWriter.Append(GetStringSize(key));
                arrayWriter.Append(key);
                arrayWriter.Append(Files[key]);
            }
        }

        protected override int GetDataSize()
        {
            int result = 0;
            var keys = Files.Keys;
            foreach(var key in keys)
            {
                result += sizeof(int) + GetStringSize(key) + sizeof(long);
            }
            return result;
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            while (!arrayReader.IsEmpty())
            {
                var fileName = arrayReader.ReadString(arrayReader.ReadInt());
                var fileSize = arrayReader.ReadLong();
                Files.Add(fileName, fileSize);
            }
        }
    }
}
