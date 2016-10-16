using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace CommunicationFramework
{
    public class NetworkingSystem
    {
        public const String LocalHost = "127.0.0.1";

        public String Port               { get; protected set; }
        public String ConectionIPAddress { get; protected set; }

        public virtual void Initialize(String port,
                                       String connectionIPAdress = null)
        {
            Port = port;
            if (String.IsNullOrEmpty(connectionIPAdress))
            {
                connectionIPAdress = GetLocalIPAddress();
            }
            ConectionIPAddress = connectionIPAdress;
            Message.Library.IsInitialized = true;
        }

        public String GetLocalIPAddress()
        {
            String result = "";
            foreach (var item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            result = ip.Address.ToString();
                            break;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(result))
                {
                    break;
                }
            }
            return result;
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
