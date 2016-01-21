using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace JacoChatServer
{
    public class Client
    {
        public TcpClient TcpClient { get; private set; }
        public string NickName { get; set; }
        public StreamReader Input { get; private set; }
        public StreamWriter Output { get; private set; }
        public string IpAddress { get { return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); } }
        public int Ping { get; set; }
        public Dictionary<string, Channel> Channels { get; private set; }

        public Client(TcpClient client)
        {
            Ping = 0;
            TcpClient = client;
            Input = new StreamReader(client.GetStream());
            Output = new StreamWriter(client.GetStream());
            Channels = new Dictionary<string, Channel>();
        }

        public void Send(string message)
        {
            Output.WriteLine(message);
            Output.Flush();
        }
    }
}

