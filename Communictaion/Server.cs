using System.Net.Sockets;
using System.Threading;

namespace CommunicationFramework
{
    public class Server : Device
    {
        public void Start()
        {
            _server = new TcpListener(IPAdress, Port);
            _server.Start();

            Logger.Log("Starting server at {0}:{1}.", IPAdress, Port);

            while (true)
            {
                OpenConnection(_server.AcceptTcpClient());
                try
                {
                    while (StayConnected())
                    {
                        WaitForMessage();
                        Thread.Sleep(100);
                    }
                }
                catch (System.Exception e)
                {
                    Logger.Log(e);
                }
                finally
                {
                    CloseConnection();
                }
            }
        }

        private TcpListener _server = null;
    }
}
