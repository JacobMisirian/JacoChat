using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace JacoChat
{
    public class JacoChatClient : IJacoChat
    {
        private NetworkStream stream;

        public JacoChatClient()
        {
        }

        public void Connect(string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);
            while (!client.Connected)
                ;
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
                byte[] data = new Byte[256];
                Int32 bytes = stream.Read(data, 0, data.Length);
                string responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                OnMessageRecieved(new MessageRecievedEventArgs { Message = responseData, Bytes = bytes } );
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
    }
}

