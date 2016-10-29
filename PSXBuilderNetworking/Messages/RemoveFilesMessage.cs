using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationFramework.Messages;

namespace PSXBuilderNetworking.Messages
{
    public class RemoveFilesMessage : FileListMessage
    {
        public RemoveFilesMessage()
            : base()
        {

        }

        public RemoveFilesMessage(List<String> files)
            : base(files)
        {

        }
    }
}
