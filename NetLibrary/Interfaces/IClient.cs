using NetLibrary.Classes;
using NetLibrary.Enums;
using NetLibrary.Models;

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
        void ConnectToServer(string ip, int port, ClientModel userInfo);

        /// <summary>
        /// Send message to other users
        /// </summary>
        /// <param name="target">User is whom need transfer message</param>
        void SendMessage(string message, ClientModel target);

        /// <summary>
        /// Send command to server
        /// </summary>
        /// <param name="commandType">Command type</param>
        /// <param name="metaData">Command metadata</param>
        void SendCommand(CommandTypes commandType, string metaData);

        /// <summary>
        /// Send request to remote/local server
        /// </summary>
        void SendPacket(Packet requestData);

        /// <summary>
        /// Close client
        /// </summary>
        void Close();
    }
}
