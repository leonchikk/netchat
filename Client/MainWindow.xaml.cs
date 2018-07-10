using System;
using System.Windows;
using static NetLibrary.Classes.Client;
using NetLibrary.Models;
using Net = NetLibrary.Classes;
using NetLibrary.Enums;
using NetLibrary.EventsArgs;

namespace Client
{
    public partial class MainWindow : Window
    {
        private Net.Client client;

        public MainWindow()
        {
            InitializeComponent();

            client = new Net.Client();

            client.OnConnected += new ConnectionHandler(delegate (object o, EventArgs a)
            {
                
            });

            client.OnDisconnected += new ConnectionHandler(delegate (object o, EventArgs a)
            {

            });

            client.OnReceiveMessage += new ReceiveMessageHandler(delegate (object sender, ReceviedMessageEventsArgs e)
            {
                
            });

            client.OnUserDisconnected += new ReceiveResponseHandler(delegate (object sender, ReceivedPacketEventsArgs e)
            {

            });

            client.OnUserJoin += new ReceiveResponseHandler(delegate (object sender, ReceivedPacketEventsArgs e)
            {

            });

            ClientModel userInfo = new ClientModel
            {
                ClientState = UserStates.Online,
                Id = Guid.NewGuid(),
                Ip = "127.0.0.1",
                Login = "Leo",
                Name = "Elijah"
            };

            client.ConnectToServer("127.0.0.1", 27015, userInfo);

        }
    }
}
