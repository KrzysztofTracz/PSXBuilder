using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommunicationFramework.Messages
{
    public class GetRootDirectoryMessage : Message
    {
        public String UserName { get; set; }

        protected override Int16 GetDataSize()
        {
            return (Int16)UserName.Length;
        }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            
        }
    }
}
