using NetLibrary.Models;
using Net = Client.Engine.Classes;

namespace Client.Engine.Models
{
    public static class CurrentConnection
    {
        public static Net.Client CurrentClient { get; set; } = new Net.Client();
        public static ClientModel CurrentClientInfo { get; set; }
        public static string ClientToken { get; set; } //Not sure
    }
}
