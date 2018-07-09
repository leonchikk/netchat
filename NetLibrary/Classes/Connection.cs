using NetLibrary.EventsArgs;
using NetLibrary.Helpers;
using NetLibrary.Models;
using System.Threading.Tasks;
using NetLibrary.States;

namespace NetLibrary.Classes
{
    public class Connection
    {
        public ConnectionModel User { get; set; }

        /// <summary>
        /// Event which invoke when client send response
        /// </summary>
        public event ReceiveHandler OnReceivedMessage;
        public delegate void ReceiveHandler(Connection sender, ReceivedPacketEventsArgs e);

        /// <summary>
        /// Event which invoke when client disconnected from server
        /// </summary>
        public event DisconnectHandler OnDisconnected;
        public delegate void DisconnectHandler(Connection sender, ReceivedPacketEventsArgs e);

        public void StartReceiveResponses()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var responseData = await NetHelper.GetDataAsync(User.TcpSocket);

                    if (responseData.ActionState == ActionState.Disconnect)
                        OnDisconnected(this, new ReceivedPacketEventsArgs(responseData));

                    if(responseData.ActionState == ActionState.Message)
                        OnReceivedMessage(this, new ReceivedPacketEventsArgs(responseData));
                }
            });
        }
    }
}
