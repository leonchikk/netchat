using NetLibrary.Models;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using Net = Client.Engine.Classes;

namespace Client.Engine.Models
{
    public static class CurrentConnection
    {
        public static Net.Client CurrentClient { get; set; } = new Net.Client();
        public static ClientModel CurrentClientInfo { get; set; }
        public static string ClientToken { get; set; } //Not sure

        public static void ParseToClientModel(JObject model)
        {
            CurrentClientInfo.Id = Guid.Parse(model["UserId"].ToString());
            CurrentClientInfo.Email = model["UserEmail"].ToString();
            CurrentClientInfo.Name = model["UserName"].ToString();
        }
    }
}
