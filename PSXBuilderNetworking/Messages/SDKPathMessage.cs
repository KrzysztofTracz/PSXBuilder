﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework;

namespace PSXBuilderNetworking.Messages
{
    public class SDKPathMessage : CommunicationFramework.Message
    {
        public String Path { get; set; }

        protected override void AppendData(ByteArrayWriter arrayWriter)
        {
            arrayWriter.Append(Path);
        }

        protected override int GetDataSize()
        {
            return GetStringSize(Path);
        }

        protected override void ReadData(ByteArrayReader arrayReader)
        {
            Path = arrayReader.ReadString();
        }
    }
}
