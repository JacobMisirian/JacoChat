using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JacoChatServer
{
    public class Client
    {
        public TcpClient TcpClient { get; private set; }
        public StreamReader Input { get; private set; }
        public StreamWriter Output { get; private set; }

        public int Ping { get; set; }

        public Dictionary<string, Channel> Channels { get; private set; }
        public Stopwatch Time { get; private set; }
        public long Idle { get { return (Time.ElapsedMilliseconds - CountedMilliseconds) / 1000; } }
        public long CountedMilliseconds { get; set; }

        public string NickName { get; set; }
        public string IpAddress { get { return ((IPEndPoint)TcpClient.Client.RemoteEndPoint).Address.ToString(); } }

        public Thread ListenForMessages { get; set; }
        public Thread SendPing { get; set; }

        public bool NetOp { get; set; }

        public Client(TcpClient client)
        {
            TcpClient = client;
            Input = new StreamReader(client.GetStream());
            Output = new StreamWriter(client.GetStream());
            Ping = 0;
            Channels = new Dictionary<string, Channel>();
            Time = new Stopwatch();
            Time.Start();
            CountedMilliseconds = 0;
            NetOp = false;
        }

        public void Send(string message)
        {
            Output.WriteLine(message);
            Output.Flush();
        }
    }
}

