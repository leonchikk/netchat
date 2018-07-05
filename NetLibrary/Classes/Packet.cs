using NetLibrary.Models;
using System;

namespace NetLibrary.Classes
{
    [Serializable()]
    public class Packet
    {
        public Client ClientInfo { get; set; }
        public ActionState ActionState { get; set; }
        public byte[] Data { get; set; }
    }
}
