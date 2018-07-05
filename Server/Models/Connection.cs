using NetLibrary.Models;
using System.Net.Sockets;

namespace Server.Models
{
    public class Connection: Client
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
