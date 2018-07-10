using NetLibrary.EventsArgs;
using NetLibrary.Helpers;
using NetLibrary.Models;
using System.Threading.Tasks;
using NetLibrary.Enums;

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
        /// Event which invoke when client send command
        /// </summary>
        public event ReceivedCommandHandler OnReceivedCommand;
        public delegate void ReceivedCommandHandler(Connection sender, ReceivedCommandEventsArgs e);

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

                    if (responseData.ActionState == ActionStates.Disconnect)
                        OnDisconnected(this, new ReceivedPacketEventsArgs(responseData));

                    if(responseData.ActionState == ActionStates.Message)
                        OnReceivedMessage(this, new ReceivedPacketEventsArgs(responseData));

                    if (responseData.ActionState == ActionStates.Command)
                        OnReceivedCommand(this, new ReceivedCommandEventsArgs(responseData.ClientInfo, responseData.Command));
                }
            });
        }
    }
}
