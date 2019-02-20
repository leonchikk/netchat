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
using System.Linq;
using System.Threading;
using System.Windows.Media.Imaging;
using AForge.Video.DirectShow;
using System.Windows.Threading;
using System.Drawing;
using System.Net.WebSockets;

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
        /// Event which invoke when client received videoframe from server
        /// </summary>
        public event ReceiveVideoFrameHandler OnReceiveVideoFrame;
        public delegate void ReceiveVideoFrameHandler(object sender, ReceviedVideoFrameEventsArgs e);

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
        /// Event which invoke when client received notification from server
        /// </summary>
        public event ReceiveNotificationHandler OnReceiveNotification;
        public delegate void ReceiveNotificationHandler(ReceiveNotificationEventsArgs e);

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

        private CancellationTokenSource _cancelRecevingPacketSource = new CancellationTokenSource();
        private CancellationToken _cancelRecevingPacketToken = CancellationToken.None;

        /// <summary>
        /// Current Interlocutor
        /// </summary>
        private ClientModel _currentVideoInterlocutor { get; set; }

        private FilterInfoCollection _captureDevices;
        /// <summary>
        /// All accessed devices
        /// </summary>
        public FilterInfoCollection CaptureDevices
        {
            get { return _captureDevices; }
        }

        private VideoCaptureDevice _captureDevice;
        /// <summary>
        /// Current selected capture device
        /// </summary>
        public VideoCaptureDevice CaptureDevice
        {
            get
            {
                return _captureDevice;
            }
            set
            {
                _captureDevice = value;
            }
        }

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
               _tcpSocket = value;
            }
            get
            {
               return _tcpSocket;
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

                if(_receivedPacket?.ActionState == ActionStates.Join && OnUserJoin != null)
                    OnUserJoin(this, new ReceivedPacketEventsArgs(_receivedPacket));

                if (_receivedPacket?.ActionState == ActionStates.Disconnect && OnUserDisconnected != null)
                    OnUserDisconnected(this, new ReceivedPacketEventsArgs(_receivedPacket));

                if (_receivedPacket?.ActionState == ActionStates.Message && OnReceiveMessage != null)
                    OnReceiveMessage(this, new ReceviedMessageEventsArgs(_receivedPacket.Conversation.Sender, _receivedPacket.Conversation.Message));

                if (_receivedPacket?.ActionState == ActionStates.CommandResult && OnReceiveCommandResult != null)
                    OnReceiveCommandResult(this, new ReceivedCommandResultsEventsArgs(_receivedPacket.Command.CommandType, _receivedPacket.Command.StatusCode, _receivedPacket.Command.Data));

                if (_receivedPacket?.ActionState == ActionStates.Notification && OnReceiveNotification != null)
                    OnReceiveNotification(new ReceiveNotificationEventsArgs(_receivedPacket.Notification));

                if (_receivedPacket?.ActionState == ActionStates.Video && OnReceiveVideoFrame != null)
                    OnReceiveVideoFrame(this, new ReceviedVideoFrameEventsArgs(_receivedPacket.Conversation.Sender, ImageHelper.ToBitmapImage(_receivedPacket.Conversation.VideoFrame)));
            }
            get
            {
               return _receivedPacket;
            }
        }
        #endregion

        #region Methods

        public Client()
        {
            _cancelRecevingPacketToken = _cancelRecevingPacketSource.Token;

            _captureDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (_captureDevices.Count > 0)
            {
                _captureDevice = new VideoCaptureDevice(_captureDevices[0].MonikerString);
                //_captureDevice.VideoResolution = VideoCapabilities
                _captureDevice.NewFrame += CaptureNewFrame;
            }
        }

        private async void CaptureNewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            GC.Collect();
            var frame = eventArgs.Frame;
            await SendVideoFrameAsync(frame, _currentVideoInterlocutor);
        }

        /// <summary>
        /// Connect to server
        /// </summary>
        public async Task TryConnectToServerAsync(string ip, int port)
        {
            try
            {
                //WebSocket socket = 

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
                    if (_cancelRecevingPacketToken.IsCancellationRequested)
                        break;

                    ReceivedPacket = await ReceiveRequestAsync();
                }
            }
            catch(Exception ex)
            {
                if (OnClosedConnection != null)
                    OnClosedConnection(this, new EventArgs());

                throw new Exception(ex.Message); //"Server has refused connection");
            }
        }

        /// <summary>
        /// Stop receiving incoming packets from server
        /// </summary>
        public void StopReceivingResponses()
        {
            _cancelRecevingPacketSource?.Cancel();
            _cancelRecevingPacketSource?.Dispose();
            _cancelRecevingPacketSource = new CancellationTokenSource();
            _cancelRecevingPacketToken = _cancelRecevingPacketSource.Token;
        }

        /// <summary>
        /// Send request to remote/local server
        /// </summary>
        public async Task SendPacketAsync(Packet requestData)
        {
            await NetHelper.SendDataAsync(_tcpSocket, requestData);
        }

        /// <summary>
        /// Send video frame to another user
        /// </summary>
        public async Task SendVideoFrameAsync(Bitmap frame, ClientModel target)
        {
            ConversationModel conversation = new ConversationModel
            {
                Sender = CurrentUser,
                Target = target,
                VideoFrame = frame
            };

            Packet messagePacket = new Packet
            {
                ActionState = ActionStates.Video,
                Conversation = conversation
            };


            await SendPacketAsync(messagePacket);
        }

        /// <summary>
        /// Send message to other user
        /// </summary>
        /// <param name="target">User is whom need transfer message</param>
        public async Task SendMessageAsync(string message, ClientModel target)
        {
            ConversationModel conversation = new ConversationModel
            {
                Sender = CurrentUser,
                Target = target,
                Message = message
            };

            Packet messagePacket = new Packet
            {
                ActionState = ActionStates.Message,
                Conversation = conversation
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

        /// <summary>
        /// Stop sending video to target user
        /// </summary>
        public async Task StopSendVideoAsync()
        {
            if (_captureDevice != null && _captureDevice.IsRunning)
            {
                _captureDevice.Stop();

                var conversation = new ConversationModel
                {
                    Target = _currentVideoInterlocutor
                };

                Packet startVideoCallPacket = new Packet
                {
                    ActionState = ActionStates.StopVideoCall,
                    Conversation = conversation
                };

                await SendPacketAsync(startVideoCallPacket);
            }
        }


        /// <summary>
        /// Send video frame to another user
        /// </summary>
        public async Task StartSendVideoAsync(ClientModel target)
        {
            if (_captureDevice != null && !_captureDevice.IsRunning)
            {
                _captureDevice.Start();
                _currentVideoInterlocutor = target;

                var conversation = new ConversationModel
                {
                    Target = target
                };

                Packet startVideoCallPacket = new Packet
                {
                    ActionState = ActionStates.StartVideoCall,
                    Conversation = conversation
                };

                await SendPacketAsync(startVideoCallPacket);
            }
        }

        #endregion
    }
}
