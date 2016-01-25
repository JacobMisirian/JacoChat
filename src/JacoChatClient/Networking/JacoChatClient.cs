using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace JacoChatClient
{
    public class JacoChatClient
    {
        private StreamWriter output;
        private StreamReader input;

        public void Connect(string ip, int port)
        {
            TcpClient client = new TcpClient(ip, port);
            input = new StreamReader(client.GetStream());
            output = new StreamWriter(client.GetStream());

            new Thread(() => beginListening()).Start();
        }

        public void Send(string message)
        {
            output.WriteLine(message);
            output.Flush();
        }

        private void beginListening()
        {
            while (true)
            {
                string responseData = input.ReadLine();
                if (responseData == "PING")
                {
                    output.WriteLine("PONG");
                    output.Flush();
                }
                else
                    OnMessageRecieved(new MessageRecievedEventArgs { Message = responseData });
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

    public class MessageRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Int32 Bytes { get { return Message.Length; } }
    }
}
