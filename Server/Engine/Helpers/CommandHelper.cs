using NetLibrary.Enums;
using NetLibrary.Models;
using Newtonsoft.Json.Linq;

namespace Server.Engine.Helpers
{
    public static class CommandHelper
    {
        public static CommandModel GetCommandResultData(CommandTypes commandType, StatusCodes statusCode, JObject data)
        {
            return new CommandModel
            {
                StatusCode = statusCode,
                CommandType = commandType,
                Data = data.ToString()
            };
        }

        public static CommandModel GetCommandResultData(CommandTypes commandType, StatusCodes statusCode, JArray data)
        {
            return new CommandModel
            {
                StatusCode = statusCode,
                CommandType = commandType,
                Data = data.ToString()
            };
        }
    }
}
