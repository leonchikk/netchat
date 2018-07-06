using NetLibrary.EventsArgs;
using NetLibrary.Models;
using System.Threading.Tasks;

namespace NetLibrary.Classes
{
    public class Connection
    {
        public ConnectionModel User { get; set; }

        /// <summary>
        /// Event which invoke when client disconnected from server
        /// </summary>
        public event ReceiveHandler OnReceivedResponse;
        public delegate void ReceiveHandler(object sender, ReceiveResponseArgs e);

        public void StartReceiveResponses()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    var responseData = await NetHelper.GetDataAsync(User.TcpSocket);

                    //Invoke event
                    OnReceivedResponse(this, new ReceiveResponseArgs(responseData));
                }
            });
        }
    }
}
