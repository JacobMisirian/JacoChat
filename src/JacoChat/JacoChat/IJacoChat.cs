using System;

namespace JacoChat
{
    public interface IJacoChat
    {
        event EventHandler<MessageRecievedEventArgs> MessageRecieved;
        event EventHandler<UserJoinedEventArgs> UserJoined;
        void Send(string message);
        void Send(byte[] data);
    }
}

