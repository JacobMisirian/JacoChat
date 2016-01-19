using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JacoChat
{
    public class JacoChatServer : IJacoChat
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
                Send(client.Stream, message);
        }

        public void Send(byte[] data)
        {
            foreach (Client client in Clients)
                Send(client.Stream, data);
        }

        public void Send(NetworkStream stream, string message)
        {
            Send(stream, System.Text.Encoding.ASCII.GetBytes(message));
        }

        public void Send(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        private void listenForClients()
        {
            while (true)
            {
                Client client = new Client(listener.AcceptTcpClient());
                Clients.Add(client);
                OnUserJoined(new UserJoinedEventArgs { Client = client } );
                new Thread(() => listenForMessagesFromUser(client)).Start();
            }
        }


        private void listenForMessagesFromUser(Client client)
        {
            var stream = client.Stream;
            while (true)
            {
                Byte[] bytes = new Byte[256];
                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    OnMessageRecieved(new MessageRecievedEventArgs { Message = data, Bytes = i, Client = client });
                }
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
}

