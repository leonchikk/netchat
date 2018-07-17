using NetLibrary.Enums;
using Newtonsoft.Json.Linq;

namespace NetLibrary.EventsArgs
{
    public class ReceivedCommandResultsEventsArgs
    {
        public CommandTypes CommandType { get; set; }
        public JObject CommandResult { get; set; }
        public StatusCodes StatusCode { get; set; }

        public ReceivedCommandResultsEventsArgs(CommandTypes commandType, StatusCodes statusCode, JObject commandResult)
        {
            CommandType = commandType;
            StatusCode = statusCode;
            CommandResult = commandResult;
        }
    }
}
