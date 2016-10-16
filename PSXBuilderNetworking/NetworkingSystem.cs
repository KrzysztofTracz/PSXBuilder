using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PSXBuilderNetworking
{
    public class NetworkingSystem : CommunicationFramework.NetworkingSystem
    {
        public const String Port = "13000";

        public String ConectionIPAddress { get; protected set; }

        public void Initialize(String connectionIPAdress = null)
        {
            CommunicationFramework.Message.Library.RegisterMessages();
            CommunicationFramework.Message.Library.IsInitialized = true;
            if(String.IsNullOrEmpty(connectionIPAdress))
            {
                connectionIPAdress = GetLocalIPAddress();
            }
            ConectionIPAddress = connectionIPAdress;
        }

        public String GetConnectionAddress()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ConectionIPAddress);
            sb.Append(":");
            sb.Append(Port);
            return sb.ToString();
        }
    }
}
