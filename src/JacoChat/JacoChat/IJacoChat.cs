using System;

namespace JacoChat
{
    public interface IJacoChat
    {
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        void Send(string message);
        void Send(byte[] data);
    }
}

