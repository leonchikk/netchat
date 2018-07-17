using NetLibrary.Classes;
using NetLibrary.Enums;
using NetLibrary.EventsArgs;
using NetLibrary.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace Client.Engine.Interfaces
{
    interface IClient
    {
        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="ip">Server Ip</param>
        /// <param name="port">Server port</param>
        Task TryConnectToServerAsync(string ip, int port);

        /// <summary>
        /// Check new receives from server
        /// </summary>
        Task StartReceivingResponsesAsync();

        /// <summary>
        /// Send message to other users
        /// </summary>
        /// <param name="target">User is whom need transfer message</param>
        Task SendMessageAsync(string message, ClientModel target);

        /// <summary>
        /// Send command to server and get response
        /// </summary>
        /// <param name="commandType">Command type</param>
        /// <param name="data">Command metadata</param>
        Task SendCommandAsync(CommandTypes commandType, JObject data);

        /// <summary>
        /// Send request to remote/local server
        /// </summary>
        Task SendPacketAsync(Packet requestData);

        /// <summary>
        /// Close client
        /// </summary>
        void Close();
    }
}
