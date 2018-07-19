using NetLibrary.Enums;
using NetLibrary.Models;
using Newtonsoft.Json.Linq;

namespace Server.Engine.Helpers
{
    public static class CommandHelper
    {
        public static CommandModel GetCommandResultData(StatusCodes statusCode, JObject data)
        {
            return new CommandModel
            {
                StatusCode = statusCode,

                Data = data.ToString()
            };
        }
    }
}
