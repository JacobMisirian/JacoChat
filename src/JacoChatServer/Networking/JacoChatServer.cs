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
            while (true)
            {
                Client client = new Client(listener.AcceptTcpClient());
                Clients.Add(client);
                new Thread(() => listenForMessagesFromUser(client)).Start();
            }
        }


        private void listenForMessagesFromUser(Client client)
        {
            var input = client.Input;
            while (true)
            {
                string message = input.ReadLine();
                OnMessageRecieved(new MessageRecievedEventArgs { Client = client, Message = message });
            }
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
}

