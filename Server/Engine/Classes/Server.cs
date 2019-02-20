using NetLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using NetLibrary.EventsArgs;
using NetLibrary.Enums;
using NetLibrary.Helpers;
using Server.Engine.Interfaces;
using NetLibrary.Classes;
using Newtonsoft.Json.Linq;
using Server.Engine.Helpers;

namespace Server.Engine.Classes
{
    public class Server : IServer
    {
        TcpListener _tcpListener;

        /// <summary>
        /// All user which connected to server
        /// </summary>
        List<Connection> CurrentConnections { get; set; } = new List<Connection>();

        /// <summary>
        /// All Notification which need transfer
        /// </summary>
        Dictionary<Guid, NotificationModel> CurrentNotifications { get; set; } = new Dictionary<Guid, NotificationModel>();

        public void Start(string ip, int port, out string error)
        {
            error = string.Empty;

            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                _tcpListener = new TcpListener(ipAddress, port);
                _tcpListener.Start();

                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        await CheckNewConnectionAsync();
                    }
                });

            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

        }

        /// <summary>
        /// Method which broadcast needed data to other client
        /// </summary>
        /// <param name="responseData">Data which need transfer to client</param>
        /// <param name="targetClient">Client which have to receive data</param>
        public void SendConversationResponseAsync(Packet responseData, List<Connection> targetClients, Connection initiator)
        {
            targetClients?.ForEach(async client => await NetHelper.SendDataAsync(client?.User?.TcpSocket, responseData));
        }

        /// <summary>
        /// Method which broadcast needed data to other client
        /// </summary>
        /// <param name="responseData">Data which need transfer to client</param>
        /// <param name="targetClient">Client which have to receive data</param>
        public async Task<bool> SendConversationResponseAsync(Packet responseData, Connection targetClient, Connection initiator)
        {
            if (targetClient == initiator)
                return false;

            return await NetHelper.SendDataAsync(targetClient?.User?.TcpSocket, responseData);
        }
        
        /// <summary>
        /// Send command result to client
        /// </summary>
        public async Task SendCommandResultAsync(Connection initiator, CommandModel commandResult)
        {
            Packet commandResultPacket = new Packet
            {
                ActionState = ActionStates.CommandResult,
                Command = commandResult
            };

            await NetHelper.SendDataAsync(initiator?.User?.TcpSocket, commandResultPacket);
        }

        public async Task SendNotificationAsync(Guid targetId, NotificationModel notification)
        {
            var target = CurrentConnections.FirstOrDefault(c => c.User.Id == targetId);

            if (target == null)
                return;

            Packet commandResultPacket = new Packet
            {
                ActionState = ActionStates.Notification,
                Notification = notification
            };

            //Add notification in query
            if (!await NetHelper.SendDataAsync(target?.User?.TcpSocket, commandResultPacket))
                CurrentNotifications.Add(targetId, notification);
        }

        /// <summary>
        /// Send response data to all users which connected to server
        /// </summary>
        /// <param name="responseData">Data which need transfer to all clients</param>
        public void BroadcastResponse(Packet responseData, Connection initiator)
        {
            CurrentConnections.ForEach(async connection => await SendConversationResponseAsync(responseData, connection, initiator));
        }

        /// <summary>
        /// Send response data to target user
        /// </summary>
        /// <param name="responseData">Data which need transfer to target client</param>
        public async Task<bool> SendMessageAsync(ConversationModel conversationData, Connection initiator)
        {
            var targetConnection = CurrentConnections.FirstOrDefault(connection => connection.User.Id == conversationData.Target.Id);
            if (targetConnection == null)
                return false;

            Packet messagePacket = new Packet
            {
                ActionState = ActionStates.Message,
                Conversation = conversationData
            };

            return await SendConversationResponseAsync(messagePacket, targetConnection, initiator);
        }

        /// <summary>
        /// Method which send video frame from one to another user
        /// </summary>
        public async Task<bool> SendVideoFrameAsync(ConversationModel conversationData, Connection initiator)
        {
            var targetConnection = CurrentConnections.FirstOrDefault(connection => connection.User.Id == conversationData.Target.Id);
            if (targetConnection == null)
                return false;

            Packet videoPacket = new Packet
            {
                ActionState = ActionStates.Video,
                Conversation = conversationData
            };

            return await SendConversationResponseAsync(videoPacket, targetConnection, initiator);
        }

        /// <summary>
        /// Check new connections from other world
        /// </summary>
        public async Task CheckNewConnectionAsync()
        {
            var newConnection = await _tcpListener.AcceptTcpClientAsync();

            var connectionUser = new ConnectionModel
            {
                TcpSocket = newConnection
            };

            var connection = new Connection
            {
                User = connectionUser
            };

            CurrentConnections.Add(connection);

            connection.OnReceivedMessage += Connection_OnReceivedMessage; ;
            connection.OnDisconnected += Connection_OnDisconnected;
            connection.OnReceivedCommand += Connection_OnReceivedCommand;
            connection.OnReceivedVideoFrame += Connection_OnReceivedVideoFrame;
            connection.OnStartSendingVideo += Connection_OnStartSendingVideo;
            connection.OnStopSendingVideo += Connection_OnStopSendingVideo;

            connection.StartReceiveResponses();
        }

        /// <summary>
        ///  Event which invoke when user start video call
        /// </summary>
        private async void Connection_OnStopSendingVideo(Connection sender, VideoCallEventArgs e)
        {
            await SendNotificationAsync(e.TargetId, new NotificationModel { NotificationType = NotificationTypes.StopVideoCall });
        }

        /// <summary>
        ///  Event which invoke when user end video call
        /// </summary>
        private async void Connection_OnStartSendingVideo(Connection sender, VideoCallEventArgs e)
        {
            await SendNotificationAsync(e.TargetId, new NotificationModel { NotificationType = NotificationTypes.StartVideoCall });
        }

        /// <summary>
        ///  Event which invoke when server receive new command from other world
        /// </summary>
        private async void Connection_OnReceivedCommand(Connection sender, ReceivedCommandEventsArgs e)
        {
            CommandModel commandResult = null;

            switch (e.Command.CommandType)
            {
                case CommandTypes.Authorization:

                    DBHelper.TryAuthorizationUser(JObject.Parse(e.Command.Data), out commandResult);
                    await SendCommandResultAsync(sender, commandResult);

                    break;

                case CommandTypes.Registration:

                    DBHelper.TryRegisterUser(JObject.Parse(e.Command.Data), out commandResult);
                    await SendCommandResultAsync(sender, commandResult);

                    break;

                case CommandTypes.UserInfo:

                    var result = DBHelper.GetUserInfo(JObject.Parse(e.Command.Data));

                    //Set Id to sender connection
                    if (result.Item2 != Guid.Empty)
                    {
                        var recentConnection = CurrentConnections.FirstOrDefault(cc => cc.User.TcpSocket == sender.User.TcpSocket);

                        if (recentConnection != null)
                            recentConnection.User.Id = result.Item2;

                        var notification = CurrentNotifications.FirstOrDefault(n => n.Key == recentConnection.User.Id).Value;

                        //Check that have user missed notification or not
                        if (notification != null)
                        {
                            await SendNotificationAsync(recentConnection.User.Id, notification);
                            CurrentNotifications.Remove(recentConnection.User.Id);
                        }
                    }

                    await SendCommandResultAsync(sender, result.Item1);
                    break;

                case CommandTypes.Search:

                    await SendCommandResultAsync(sender, DBHelper.GetSearchResults(JObject.Parse(e.Command.Data)));
                    break;

                case CommandTypes.GetContacts:

                    await SendCommandResultAsync(sender, DBHelper.GetContacts(JObject.Parse(e.Command.Data)));
                    break;

                case CommandTypes.SendToApproveContact:

                    result = DBHelper.AddNewContact(JObject.Parse(e.Command.Data));

                    if (result.Item2 != Guid.Empty)
                        await SendNotificationAsync(result.Item2, new NotificationModel { NotificationType = NotificationTypes.NewContact });

                    await SendCommandResultAsync(sender, result.Item1);
                    break;

                case CommandTypes.RemoveContact:

                    result = DBHelper.RemoveContact(JObject.Parse(e.Command.Data));

                    if (result.Item2 != Guid.Empty)
                        await SendNotificationAsync(result.Item2, new NotificationModel { NotificationType = NotificationTypes.RemoveContact });

                    await SendCommandResultAsync(sender, result.Item1);
                    break;

                case CommandTypes.ApproveContact:

                    result = DBHelper.ApproveContact(JObject.Parse(e.Command.Data));

                    if (result.Item2 != Guid.Empty)
                        await SendNotificationAsync(result.Item2, new NotificationModel { NotificationType = NotificationTypes.ApprovedContact });

                    await SendCommandResultAsync(sender, result.Item1);
                    break;
            }
        }

        /// <summary>
        /// Event which invoke when server receive new response from other world
        /// </summary>
        private void Connection_OnDisconnected(Connection sender, ReceivedPacketEventsArgs e)
        {
            CloseConnection(sender);
            CurrentConnections.Remove(sender);
        }

        /// <summary>
        /// Event which invoke when server receive new response from other world
        /// </summary>
        private async void Connection_OnReceivedMessage(Connection sender, ReceivedPacketEventsArgs e)
        {
            if (!await SendMessageAsync(e.ReceivedPacket.Conversation, sender))
                CurrentNotifications.Add(e.ReceivedPacket.Conversation.Target.Id, new NotificationModel { NotificationType = NotificationTypes.Message });
        }

        /// <summary>
        ///  Event which invoke when server receive video frame from one user
        /// </summary>
        private async void Connection_OnReceivedVideoFrame(Connection sender, ReceivedPacketEventsArgs e)
        {
            await SendVideoFrameAsync(e.ReceivedPacket.Conversation, sender);
        }

        /// <summary>
        /// Method which call when user disconnect
        /// </summary>
        public void CloseConnection(Connection initiator)
        {
            initiator?.User?.TcpSocket?.Close();
        }
    }
}
