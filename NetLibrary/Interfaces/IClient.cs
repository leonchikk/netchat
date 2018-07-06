using NetLibrary.Classes;
using System;
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
        void ConnectToServer(string ip, int port, out string error);

        /// <summary>
        /// Send async request to server
        /// </summary>
        /// <param name="requestData">Data which need transfer to server</param>
        void SendRequestAsync(Packet requestData, out string error);

        /// <summary>
        /// Receive async request from server
        /// </summary>
        /// <returns>Data array which returns server</returns>
        Task<Packet> ReceiveRequestAsync();
    }
}
