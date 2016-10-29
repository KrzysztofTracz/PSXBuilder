using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;
using CommunicationFramework.Messages;

namespace PSXBuilderNetworking.Messages
{
    public class ProjectFileUploadMessage : FileUploadMessage
    {
        public bool Compile { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(Compile);
            base.AppendData(arrayWriter);
        }

        protected override int GetDataSize()
        {
            return sizeof(bool) + base.GetDataSize();
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Compile = arrayReader.ReadBool();
            base.ReadData(arrayReader);
        }
    }
}
