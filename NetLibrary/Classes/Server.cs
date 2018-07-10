using NetLibrary.Interfaces;
using NetLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Sockets;
using System.Net;
using NetLibrary.EventsArgs;
using NetLibrary.Enums;
using NetLibrary.Helpers;

namespace NetLibrary.Classes
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
                        await CheckNewConnection();
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
        public void SendResponse(Packet responseData, List<Connection> targetClients, Connection initiator)
        {
            targetClients?.ForEach(client => NetHelper.SendDataAsync(client?.User?.TcpSocket, responseData));
        }

        /// <summary>
        /// Method which broadcast needed data to other client
        /// </summary>
        /// <param name="responseData">Data which need transfer to client</param>
        /// <param name="targetClient">Client which have to receive data</param>
        public void SendResponse(Packet responseData, Connection targetClient, Connection initiator)
        {
            if (targetClient == initiator)
                return;

            NetHelper.SendDataAsync(targetClient?.User?.TcpSocket, responseData);
        }

        /// <summary>
        /// Send response data to all users which connected to server
        /// </summary>
        /// <param name="responseData">Data which need transfer to all clients</param>
        public void BroadcastResponse (Packet responseData, Connection initiator)
        {
            CurrentConnections.ForEach(connection => SendResponse(responseData, connection, initiator));
        }

        /// <summary>
        /// Check new connections from other world
        /// </summary>
        public async Task CheckNewConnection()
        {
            var newConnection = await _tcpListener.AcceptTcpClientAsync();
            var response = await NetHelper.GetDataAsync(newConnection);

            if (response.ActionState == ActionStates.Connect)
            {
                var clientInfo = response.ClientInfo;

                var connectionUser = new ConnectionModel
                {
                    ClientState = clientInfo.ClientState,
                    Id = clientInfo.Id,
                    Ip = clientInfo.Ip,
                    Login = clientInfo.Login,
                    Name = clientInfo.Name,
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
        }

        /// <summary>
        ///  Event which invoke when server receive new command from other world
        /// </summary>
        private void Connection_OnReceivedCommand(Connection sender, ReceivedCommandEventsArgs e)
        {
           
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
