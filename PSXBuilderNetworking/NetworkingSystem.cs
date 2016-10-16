using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilderNetworking
{
    public class NetworkingSystem : CommunicationFramework.NetworkingSystem
    {
        public void Initialize(String connectionIPAdress = null)
        {          
            CommunicationFramework.Message.Library.RegisterMessages();
            base.Initialize("13000", connectionIPAdress);
        }
    }
}
