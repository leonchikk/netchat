using NetLibrary.Classes;
using NetLibrary.Models;
using System.Threading.Tasks;

namespace NetLibrary.Interfaces
{
    interface IServer
    {
        /// <summary>
        /// Start server
        /// </summary>
        void Start(string ip, int port, out string error);

        /// <summary>
        /// Method which broadcast needed data to other client
        /// </summary>
        /// <param name="responseData">Data which need transfer to client</param>
        /// <param name="targetClient">Client which have to receive data</param>
        void SendResponse(Packet responseData, Connection targetClient, Connection initiator);

        /// <summary>
        /// Send response data to all users which connected to server
        /// </summary>
        /// <param name="responseData">Data which need transfer to all clients</param>
        void BroadcastResponse(Packet responseData, Connection initiator);

        void CloseConnection(Connection initiator);

        Task CheckNewConnection();
    }
}
