using System;
using System.Net;
using System.Net.Sockets;

namespace JacoChat
{
    public class Client
    {
        public TcpClient TcpClient { get; private set; }
        public NetworkStream Stream { get { return TcpClient.GetStream(); } }
        public string IpAddress { get { return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); } }

        public Client(TcpClient client)
        {
            TcpClient = client;
        }
    }
}

