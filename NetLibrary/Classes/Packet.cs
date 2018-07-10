using NetLibrary.Models;
using NetLibrary.Enums;
using System;

namespace NetLibrary.Classes
{
    [Serializable()]
    public class Packet
    {
        public ClientModel ClientInfo { get; set; }
        public ActionStates ActionState { get; set; }
        public ConversationModel Conversation { get; set; }
        public CommandModel Command { get; set; }
        public string SystemMessage { get; set; }
    }
}
