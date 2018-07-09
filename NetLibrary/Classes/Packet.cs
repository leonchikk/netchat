using NetLibrary.Models;
using NetLibrary.States;
using System;

namespace NetLibrary.Classes
{
    [Serializable()]
    public class Packet
    {
        public ClientModel ClientInfo { get; set; }
        public ActionState ActionState { get; set; }
        public ConversationModel Conversation { get; set; }
        public string SystemMessage { get; set; }
    }
}
