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
        private TcpListener listener;
        private NetworkStream stream;

        public JacoChatServer()
        {
        }

        public void Listen(int port)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            stream = client.GetStream();

            new Thread(() => beginListening()).Start();
        }

        public void Send(string message)
        {
            Send(System.Text.Encoding.ASCII.GetBytes(message));
        }

        public void Send(byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }

        private void beginListening()
        {
            while (true)
            {
                Byte[] bytes = new Byte[256];
                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    var data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                    OnMessageRecieved(new MessageRecievedEventArgs { Message = data, Bytes = i } );
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
    }
}

