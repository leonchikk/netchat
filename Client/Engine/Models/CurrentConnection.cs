using Net = Client.Engine.Classes;

namespace Client.Engine.Models
{
    public static class CurrentConnection
    {
        public static Net.Client CurrentClient { get; set; } = new Net.Client();
    }
}
