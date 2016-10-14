using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilderNetworking
{
    public class System
    {
        public static void Initialize()
        {
            CommunicationFramework.Message.Library.RegisterMessages();
            CommunicationFramework.Message.Library.IsInitialized = true;
        }
    }
}
