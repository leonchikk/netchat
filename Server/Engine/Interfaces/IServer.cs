using NetLibrary.Classes;
using NetLibrary.Models;
using Server.Engine.Classes;
using System;
using System.Threading.Tasks;

namespace Server.Engine.Interfaces
{
    interface IServer
    {
        /// <summary>
        /// Start server
        /// </summary>
        void Start(string ip, int port, out string error);

        /// <summary>
        /// Send command result to client
        /// </summary>
        Task SendCommandResultAsync(Connection initiator, CommandModel commandResult);

        /// <summary>
        /// Send notification to client
        /// </summary>
        Task SendNotificationAsync(Guid targterId, NotificationModel notification);

        /// <summary>
        /// Method which broadcast needed data to other client
        /// </summary>
        /// <param name="responseData">Data which need transfer to client</param>
        /// <param name="targetClient">Client which have to receive data</param>
        Task SendConversationResponseAsync(Packet responseData, Connection targetClient, Connection initiator);

        /// <summary>
        /// Send response data to all users 
        /// </summary>
        /// <param name="responseData">Data which need transfer to all clients</param>
        void BroadcastResponse(Packet responseData, Connection initiator);

        /// <summary>
        /// Send response data to target user
        /// </summary>
        /// <param name="responseData">Data which need transfer to target client</param>
        Task SendMessage(ConversationModel responseData, Connection initiator);

        void CloseConnection(Connection initiator);

        Task CheckNewConnectionAsync();
    }
}
