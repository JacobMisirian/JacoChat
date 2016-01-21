using System;

namespace JacoChatServer
{
    public interface IJacoChat
    {
        void Send(string message);
        void Send(byte[] data);
    }
}