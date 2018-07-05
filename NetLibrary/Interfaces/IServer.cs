using NetLibrary.Models;

namespace NetLibrary.Interfaces
{
    interface IServer
    {
        /// <summary>
        /// Method which broadcast needed data to other client
        /// </summary>
        /// <param name="broadcastData">Data which need transfer to client</param>
        /// <param name="targetClient">Client which have to receive data</param>
        void BroadcastRequest(byte[] broadcastData, Client targetClient);

        void CloseConnection(Client initiator);

        void CheckNewConnection();
    }
}
