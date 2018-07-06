using NetLibrary.Models;
using System;

namespace NetLibrary.Classes
{
    [Serializable()]
    public class Packet
    {
        public ClientModel ClientInfo { get; set; }
        public ActionState ActionState { get; set; }
        public string Message { get; set; }
    }
}
