using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetLibrary.Interfaces;
using NetLibrary.EventsArgs;

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
        /// Event which invoke when client received response from server
        /// </summary>
        public event ReceiveResponseHandler OnReceivedResponse;
        public delegate void ReceiveResponseHandler(object sender, ReceiveResponseArgs e);

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
                //Invoke event
                OnReceivedResponse(this, new ReceiveResponseArgs(value));
            }
            get
            {
               return _receivedPacket;
            }
        }

        public void ConnectToServer(string ip, int port, out string error)
        {
            error = string.Empty;

            try
            {
                _tcpSocket = new TcpClient(ip, port);

                //Invoke event
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
                error = ex.Message;

                //Invoke event
                OnDisconnected(this, new EventArgs());
            }
        }

        public async Task<Packet> ReceiveRequestAsync()
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

        public void SendRequestAsync(Packet requestData, out string error)
        {
            error = string.Empty;

            try
            {
                Task.Run(() => NetHelper.SendDataAsync(_tcpSocket, requestData));
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
        }
    }
}
