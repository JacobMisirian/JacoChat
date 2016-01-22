using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JacoChatServer
{
    public class JacoChatServer
    {
        public List<Client> Clients = new List<Client>();
        private TcpListener listener;

        public void Listen(string ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            listener.Start();

            new Thread(() => listenForClients()).Start();
        }

        public void Send(string message)
        {
            foreach (Client client in Clients)
            {
                client.Output.WriteLine(message);
                client.Output.Flush();
            }
        }
       
        private void listenForClients()
        {
            Client client = null;
            while (true)
            {
                try
                {
                    client = new Client(listener.AcceptTcpClient());
                    Clients.Add(client);
                    new Thread(() => listenForMessagesFromUser(client)).Start();
                    new Thread(() => sendPing(client)).Start();
                }
                catch (IOException ex)
                {
                    MainClass.ProcessOutput(ex.Message);
                    OnUserDisconnected(new UserDisconnectedEventArgs { Client = client, Reason = ex.Message });
                }

                Thread.Sleep(20);
            }
        }

        private void listenForMessagesFromUser(Client client)
        {
            try
            {
                while (true)
                {
                    string message = client.Input.ReadLine();
                    if (message == "PONG")
                        client.Ping = 0;
                    else
                        OnMessageRecieved(new MessageRecievedEventArgs { Client = client, Message = message });
                    Thread.Sleep(20);
                }
            }
            catch (IOException ex)
            {
                MainClass.ProcessOutput(ex.Message);
                OnUserDisconnected(new UserDisconnectedEventArgs { Client = client, Reason = ex.Message });
            }
        }

        private void sendPing(Client client)
        {
            try
            {
                while (client.Ping <= 10000)
                {
                    client.Output.WriteLine("PING");
                    client.Output.Flush();
                    client.Ping += 1000;
                    Thread.Sleep(1000);
                }
            }
            catch (IOException ex)
            {
                MainClass.ProcessOutput(ex.Message);
                OnUserDisconnected(new UserDisconnectedEventArgs { Client = client, Reason = ex.Message });
            }
            OnUserDisconnected(new UserDisconnectedEventArgs { Client = client, Reason = "Ping Timeout: 10 Seconds" } );
        }

        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        protected virtual void OnMessageRecieved(MessageRecievedEventArgs e)
        {
            EventHandler<MessageRecievedEventArgs> handler = MessageRecieved;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<UserJoinedEventArgs> UserJoined;
        protected virtual void OnUserJoined(UserJoinedEventArgs e)
        {
            EventHandler<UserJoinedEventArgs> handler = UserJoined;
            if (handler != null)
                handler(this, e);
        }

        public event EventHandler<UserDisconnectedEventArgs> UserDisconnected;
        protected virtual void OnUserDisconnected(UserDisconnectedEventArgs e)
        {
            EventHandler<UserDisconnectedEventArgs> handler = UserDisconnected;
            if (handler != null)
                handler(this, e);
        }
    }

    public class MessageRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Int32 Bytes { get { return Message.Length; } }
        public Client Client { get; set; }
    }

    public class UserJoinedEventArgs : EventArgs
    {
        public Client Client { get; set; }
    }

    public class UserDisconnectedEventArgs : EventArgs
    {
        public Client Client { get; set; }
        public string Reason { get; set; }
    }
}

