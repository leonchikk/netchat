using NetLibrary.Models;

namespace NetLibrary.EventsArgs
{
    public class ReceviedMessageEventsArgs
    {
        public ClientModel Sender { get; set; }
        public string Message { get; set; }

        public ReceviedMessageEventsArgs(ClientModel sender, string message)
        {
            Sender = sender;
            Message = message;
        }
    }
}
