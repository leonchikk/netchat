﻿using System.Net.Sockets;

namespace NetLibrary.Models
{
    public class ConnectionModel: ClientModel
    {
        private TcpClient _tcpSocket;
        ///<summary>
        ///Client connection
        ///</summary>
        public TcpClient TcpSocket
        {
            get { return _tcpSocket; }
            set
            {
                _tcpSocket = value;
                OnPropertyChanged("TcpSocket");
            }
        }
    }
}