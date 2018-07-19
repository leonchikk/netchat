using NetLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
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
            catch(Exception ex)
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
        public async Task SendConversationResponseAsync(Packet responseData, Connection targetClient, Connection initiator)
        {
            if (targetClient == initiator)
                return;

            await NetHelper.SendDataAsync(targetClient?.User?.TcpSocket, responseData);
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

        /// <summary>
        /// Send response data to all users which connected to server
        /// </summary>
        /// <param name="responseData">Data which need transfer to all clients</param>
        public void BroadcastResponse (Packet responseData, Connection initiator)
        {
            CurrentConnections.ForEach(async connection => await SendConversationResponseAsync(responseData, connection, initiator));
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

            connection.StartReceiveResponses();
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
                    
                    await SendCommandResultAsync(sender, DBHelper.GetUserInfo(JObject.Parse(e.Command.Data)));

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
        private void Connection_OnReceivedMessage(Connection sender, ReceivedPacketEventsArgs e)
        {
            BroadcastResponse(e.ReceivedPacket, sender);
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
