using System;

namespace JacoChat
{
    public class MessageRecievedEventArgs : EventArgs
    {
        public string Message { get; set; }
        public Int32 Bytes { get; set; }
        public Client Client { get; set; }
    }

    public class UserJoinedEventArgs : EventArgs
    {
        public Client Client { get; set; }
    }
}

