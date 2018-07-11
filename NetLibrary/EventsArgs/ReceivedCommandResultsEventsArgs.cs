using NetLibrary.Enums;
using Newtonsoft.Json.Linq;

namespace NetLibrary.EventsArgs
{
    public class ReceivedCommandResultsEventsArgs
    {
        public CommandTypes CommandType { get; set; }
        public JObject CommandResult { get; set; }

        public ReceivedCommandResultsEventsArgs(CommandTypes commandType, JObject commandResult)
        {
            CommandType = commandType;
            CommandResult = commandResult;
        }
    }
}
