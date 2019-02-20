using NetLibrary.Classes;
using NetLibrary.Enums;
using NetLibrary.Models;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

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
        /// Send video to target user
        /// </summary>
        Task StartSendVideoAsync(ClientModel target);

        /// <summary>
        /// Stop sending video to target user
        /// </summary>
        Task StopSendVideoAsync();

        /// <summary>
        /// Send video frame to another user
        /// </summary>
        Task SendVideoFrameAsync(Bitmap frame, ClientModel target);

        /// <summary>
        /// Send command to server and get response
        /// </summary>
        /// <param name="commandType">Command type</param>
        /// <param name="data">Command metadata</param>
        Task SendCommandAsync(CommandTypes commandType, JObject data);

        /// <summary>
        /// Send request to server
        /// </summary>
        Task SendPacketAsync(Packet requestData);

        /// <summary>
        /// Close client
        /// </summary>
        void Close();
    }
}
