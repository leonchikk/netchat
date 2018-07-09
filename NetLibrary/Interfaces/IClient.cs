using NetLibrary.Classes;
using NetLibrary.Models;
using System;
using System.Collections.Generic;

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
        void ConnectToServer(string ip, int port);

        /// <summary>
        /// Send message to other users
        /// </summary>
        /// <param name="target">User is whom need transfer message</param>
        void SendMessage(string message, ClientModel sender, ClientModel target);

        /// <summary>
        /// Send request to remote/local server
        /// </summary>
        void SendPacket(Packet requestData);

        /// <summary>
        /// Send connection to server
        /// </summary>
        void SendConnectionInfo(ClientModel clientInfo);
    }
}
