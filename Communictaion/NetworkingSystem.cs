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
    }
}
