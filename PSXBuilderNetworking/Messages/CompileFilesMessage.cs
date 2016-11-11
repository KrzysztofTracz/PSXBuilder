using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework.Messages;

namespace PSXBuilderNetworking.Messages
{
    public class CompileFilesMessage : FileListMessage
    {
        public CompileFilesMessage()
            : base()
        {

        }

        public CompileFilesMessage(List<String> files)
            : base(files)
        {

        }
    }
}