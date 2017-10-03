using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Tornado14.Utils.Net
{
    public class ConnectorTcp : ConnectorBase
    {
        TcpClient client = new TcpClient();
        public TcpClient Client
        {
            get
            {
                return client;
            }
        }
        IPEndPoint serverEndPoint;
        public NetworkStream clientStream;

        Package package = new Package();

        const int buffer_len = 21;
        byte[] recv_buffer = new byte[buffer_len];
        byte[] send_buffer = new byte[buffer_len];


        IHubProxy hubProxy;

        public ConnectorTcp(IPAddress address)
        {
            HubConnection hubConnection;
            serverEndPoint = new IPEndPoint(address, 3000);
            hubConnection = new HubConnection("http://localhost:55449/");
            hubProxy = hubConnection.CreateHubProxy("ChatHub");
            hubProxy.On<string, string>("broadcastMessage", (name, message) =>
            {
                Console.WriteLine("Incoming data: {0} {1}", name, message);
            });
            ServicePointManager.DefaultConnectionLimit = 10;
            hubConnection.Start().Wait();
        }
        public ConnectorTcp(TcpClient client)
        {
            this.client = client;

            LingerOption linger = new LingerOption(true, 1);
            client.LingerState = linger;
            clientStream = this.client.GetStream();
        }

        public override void Connect()
        {
            LingerOption linger = new LingerOption(true, 1);
            client.NoDelay = false;
            client.LingerState = linger;
            client.Connect(serverEndPoint);
            clientStream = client.GetStream();
        }

        public override void Close()
        {
            clientStream.Close();
            client.Close();
        }
        public override bool Send(Package package)
        {
            hubProxy.Invoke("send", "TCP t14Lab.MessageTree.Connector", XmlSerializationHelper.Serialize(package)).Wait();
            byte[] data = Encoding.UTF8.GetBytes(XmlSerializationHelper.Serialize(package));
            clientStream.Write(data, 0, data.Length);
            return true;
        }

        string recivedData = string.Empty;
        public override bool Receive(out Package result)
        {
            bool ok = false;
            result = new Package();
            
            if (clientStream.DataAvailable)
            {
                byte[] buffer = new byte[123];
                int numberOfBytesRead = 0;
                do
                {
                    numberOfBytesRead = clientStream.Read(buffer, 0, buffer.Length);
                    recivedData = String.Join(string.Empty, new String[] { recivedData, Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead) });

                    int end = recivedData.IndexOf("</Package>");
                    if (end > 0)
                    {
                        end += 10;
                        ok = true;
                        string packageString = recivedData.Substring(0, end);
                        recivedData = recivedData.Substring(end, recivedData.Length - end);
                        result = XmlSerializationHelper.Desirialize<Package>(packageString);
                        
                        return ok;
                    }
                }
                while (clientStream.DataAvailable);
            }
            
            return ok;
        }

    }
}
