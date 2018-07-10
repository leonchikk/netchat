
using NetLibrary.Models;

namespace NetLibrary.EventsArgs
{
    public class ReceivedCommandEventsArgs
    {
        public ClientModel Sender { get; set; }
        public CommandModel Command { get; set; }

        public ReceivedCommandEventsArgs(ClientModel sender, CommandModel command)
        {
            Sender = sender;
            Command = command;
        }
    }
}
