using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetLibrary.EventsArgs;
using NetLibrary.Helpers;
using NetLibrary.Enums;
using NetLibrary.Models;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Client.Engine.Interfaces;
using NetLibrary.Classes;
using System.Timers;

namespace Client.Engine.Classes
{
    public class Client : IClient
    {
        #region Events
        /// <summary>
        /// Event which invoke when client connected to server
        /// </summary>
        public event ConnectionHandler OnConnected;

        /// <summary>
        /// Event which invoke when establish connection to server
        /// </summary>
        public event ConnectionHandler OnEstablishConnection;

        /// <summary>
        /// Event which invoke when client disconnected from server
        /// </summary>
        public event ConnectionHandler OnClosedConnection;
        public delegate void ConnectionHandler(object sender, EventArgs e);
        
        /// <summary>
        /// Event which invoke when client received message from server
        /// </summary>
        public event ReceiveMessageHandler OnReceiveMessage;
        public delegate void ReceiveMessageHandler(object sender, ReceviedMessageEventsArgs e);

        /// <summary>
        /// Event which invoke when client received message from server
        /// </summary>
        public event ReceiveCommandResultHandler OnReceiveCommandResult;
        public delegate void ReceiveCommandResultHandler(object sender, ReceivedCommandResultsEventsArgs e);

        /// <summary>
        /// Event which invoke when new user disconnected from server
        /// </summary>
        public event ReceiveResponseHandler OnUserDisconnected;

        /// <summary>
        /// Event which invoke when new user connected to server
        /// </summary>
        public event ReceiveResponseHandler OnUserJoin;
        public delegate void ReceiveResponseHandler(object sender, ReceivedPacketEventsArgs e);
        #endregion

        #region Properties
        private object _lockTcpSocket;

        private Timer _connectionTimer;

        private ClientModel _currentUser;
        /// <summary>
        /// Current user 
        /// </summary>
        public ClientModel CurrentUser
        {
            get
            {
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        } 

        private TcpClient _tcpSocket;
        /// <summary>
        /// Current client connection
        /// </summary>
        public  TcpClient TcpSocket
        {
            set
            {
                lock (_lockTcpSocket)
                {
                    _tcpSocket = value;
                }
            }
            get
            {
                lock (_lockTcpSocket)
                {
                    return _tcpSocket;
                }
            }
        }

        private Packet _receivedPacket;
        /// <summary>
        /// Received packet from server
        /// </summary>
        public Packet ReceivedPacket
        {
            set
            {
                _receivedPacket = value;

                if(_receivedPacket?.ActionState == ActionStates.Connect && OnUserJoin != null)
                    OnUserJoin(this, new ReceivedPacketEventsArgs(_receivedPacket));

                if (_receivedPacket?.ActionState == ActionStates.Disconnect && OnUserDisconnected != null)
                    OnUserDisconnected(this, new ReceivedPacketEventsArgs(_receivedPacket));

                if (_receivedPacket?.ActionState == ActionStates.Message && OnReceiveMessage != null)
                    OnReceiveMessage(this, new ReceviedMessageEventsArgs(_receivedPacket.Conversation.Sender, _receivedPacket.Conversation.Message));

                if (_receivedPacket?.ActionState == ActionStates.CommandResult && OnReceiveCommandResult != null)
                    OnReceiveCommandResult(this, new ReceivedCommandResultsEventsArgs(_receivedPacket.Command.CommandType, _receivedPacket.Command.StatusCode, JObject.Parse(_receivedPacket.Command.Data)));

            }
            get
            {
               return _receivedPacket;
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Connect to remote/local server
        /// </summary>
        public async Task TryConnectToServerAsync(string ip, int port)
        {
            try
            {
                _tcpSocket = new TcpClient();
                await _tcpSocket.ConnectAsync(ip, port);

                if (OnConnected != null)
                    OnConnected(this, new EventArgs());
            }
            catch (Exception)
            {
                throw new Exception("Could not connect to server");
            }
        }

        /// <summary>
        /// Check new receives from server
        /// </summary>
        public async Task StartReceivingResponsesAsync()
        {
            try
            {
                while (true)
                {
                    ReceivedPacket = await ReceiveRequestAsync();
                }
            }
            catch(Exception)
            {
                if (OnClosedConnection != null)
                    OnClosedConnection(this, new EventArgs());

                throw new Exception("Server has refused connection");
            }
        }

        /// <summary>
        /// Send request to remote/local server
        /// </summary>
        public async Task SendPacketAsync(Packet requestData)
        {
            try
            {
                await NetHelper.SendDataAsync(_tcpSocket, requestData);
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// Send message to other user
        /// </summary>
        /// <param name="target">User is whom need transfer message</param>
        public async Task SendMessageAsync(string message, ClientModel target)
        {
            Packet messagePacket = new Packet
            {
                ActionState = ActionStates.Message,
                ClientInfo = target,
                Conversation = new ConversationModel
                {
                    Sender = CurrentUser,
                    Target = target,
                    Message = message
                }
            };

            await SendPacketAsync(messagePacket);
        }

        /// <summary>
        /// Send command to server and get response
        /// </summary>
        /// <param name="commandType">Command type</param>
        /// <param name="metaData">Command metadata</param>
        public async Task SendCommandAsync(CommandTypes commandType, JObject commandData)
        {
            CommandModel command = new CommandModel
            {
                Data = commandData.ToString(),
                CommandType = commandType
            };

            Packet commandPacket = new Packet
            {
                ActionState = ActionStates.Command,
                Command = command
            };

            await SendPacketAsync(commandPacket);
        }

        public void Close()
        {
            TcpSocket?.Close();
        }

        /// <summary>
        /// Receive response data from server
        /// </summary>
        private async Task<Packet> ReceiveRequestAsync()
        {
            try
            {
                return await NetHelper.GetDataAsync(_tcpSocket);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}
