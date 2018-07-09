using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetLibrary.Interfaces;
using NetLibrary.EventsArgs;
using NetLibrary.Helpers;
using NetLibrary.States;
using NetLibrary.Models;
using System.Collections.Generic;

namespace NetLibrary.Classes
{
    public class Client : IClient
    {
        private object _lockTcpSocket;

        /// <summary>
        /// Event which invoke when client connected to server
        /// </summary>
        public event ConnectionHandler OnConnected;

        /// <summary>
        /// Event which invoke when client disconnected from server
        /// </summary>
        public event ConnectionHandler OnDisconnected;
        public delegate void ConnectionHandler(object sender, EventArgs e);
        
        /// <summary>
        /// Event which invoke when client received message from server
        /// </summary>
        public event ReceiveMessageHandler OnReceiveMessage;
        public delegate void ReceiveMessageHandler(object sender, ReceviedMessageEventsArgs e);

        /// <summary>
        /// Event which invoke when new user disconnected from server
        /// </summary>
        public event ReceiveResponseHandler OnUserDisconnected;

        /// <summary>
        /// Event which invoke when new user connected to server
        /// </summary>
        public event ReceiveResponseHandler OnUserJoin;
        public delegate void ReceiveResponseHandler(object sender, ReceivedPacketEventsArgs e);

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

                if(_receivedPacket?.ActionState == ActionState.Connect)
                    OnUserJoin(this, new ReceivedPacketEventsArgs(_receivedPacket));

                if (_receivedPacket?.ActionState == ActionState.Disconnect)
                    OnUserDisconnected(this, new ReceivedPacketEventsArgs(_receivedPacket));

                if (_receivedPacket?.ActionState == ActionState.Message)
                    OnReceiveMessage(this, new ReceviedMessageEventsArgs(_receivedPacket.Conversation.Sender, _receivedPacket.Conversation.Message));
            }
            get
            {
               return _receivedPacket;
            }
        }

        /// <summary>
        /// Connect to remote/local server
        /// </summary>
        public void ConnectToServer(string ip, int port)
        {
            try
            {
                _tcpSocket = new TcpClient(ip, port);

                OnConnected(this, new EventArgs());

                //Check new receives from server
                Task.Factory.StartNew(async () =>
                {
                    while (true)
                    {
                        ReceivedPacket = await ReceiveRequestAsync();
                    }
                });
            }
            catch (Exception ex)
            {
                OnDisconnected(this, new EventArgs());
            }
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
            catch(Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Send request to remote/local server
        /// </summary>
        public void SendPacket(Packet requestData)
        {
            try
            {
                Task.Run(() => NetHelper.SendDataAsync(_tcpSocket, requestData));
            }
            catch (Exception){ }
        }

        /// <summary>
        /// Send message to other user
        /// </summary>
        /// <param name="target">User is whom need transfer message</param>
        public void SendMessage(string message, ClientModel sender, ClientModel target)
        {
            Packet messagePacket = new Packet
            {
                ActionState = ActionState.Message,
                ClientInfo = target,
                Conversation = new ConversationModel
                {
                    Sender = sender,
                    Target = target,
                    Message = message
                }
            };

            SendPacket(messagePacket);
        }

        /// <summary>
        /// SendConnectionInfoServer
        /// </summary>
        public void SendConnectionInfo(ClientModel clientInfo)
        {
            Packet info = new Packet
            {
                ActionState = ActionState.Connect,
                ClientInfo = clientInfo
            };

            SendPacket(info);

        }
    }
}
