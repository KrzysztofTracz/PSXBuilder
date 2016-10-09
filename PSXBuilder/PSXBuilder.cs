using System;
using System.Net.Sockets;
using System.Text;
using ApplicationFramework;

namespace PSXBuilder
{
    class PSXBuilder
    {
        public enum Settings
        {
            PSXBuildMachineAddress,
            PSXSDKPath
        }

        static int Main(String[] args)
        {
            var application = new Application("PSXBuilder");
            var result = application.Start(args);

            return result ? 0 : -1;

            var message = "no elo!";

            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = 13000;
                TcpClient client = new TcpClient("192.168.0.13", port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                application.Console.WriteLine("Sent: {0}", message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                application.Console.WriteLine("Received: {0}", responseData);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                application.Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                application.Console.WriteLine("SocketException: {0}", e);
            }

            application.Console.WriteLine("\n Press Enter to continue...");
            System.Console.Read();
        }
    }
}
