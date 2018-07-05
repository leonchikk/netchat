
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NetLibrary.Interfaces
{
    interface IClient
    {
        /// <summary>
        /// Connect to server
        /// </summary>
        /// <param name="ip">Server Ip</param>
        /// <param name="port">Server port</param>
        /// <param name="error">Error which can happed during connection</param>
        /// <returns></returns>
        Task<TcpClient> ConnectToServer(string ip, string port, out string error);

        /// <summary>
        /// Send request to server
        /// </summary>
        /// <param name="requestData">Data which need transfer to server</param>
        Task SendRequest(byte[] requestData);

        /// <summary>
        /// Receive request from server
        /// </summary>
        /// <returns>Data array which returns server</returns>
        Task<byte[]> ReceiveRequest();
    }
}
