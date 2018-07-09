using System;
using System.Windows;
using static NetLibrary.Classes.Client;
using NetLibrary.Models;
using Net = NetLibrary.Classes;
using NetLibrary.States;
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
                ClientModel info = new ClientModel
                {
                    ClientState = UserState.Online,
                    Id = Guid.NewGuid(),
                    Ip = "127.0.0.1",
                    Login = "Leo",
                    Name = "Elijah"
                };
                
                client.SendConnectionInfo(info);
            });

            client.OnDisconnected += new ConnectionHandler(delegate (object o, EventArgs a)
            {

            });

            client.OnReceiveMessage += new ReceiveMessageHandler(delegate (object sender, RecevieMessageEventsArgs e)
            {
                
            });

            client.OnUserDisconnected += new ReceiveResponseHandler(delegate (object sender, ReceivePacketEventsArgs e)
            {

            });

            client.OnUserJoin += new ReceiveResponseHandler(delegate (object sender, ReceivePacketEventsArgs e)
            {

            });

            client.ConnectToServer("127.0.0.1", 27015);

        }
    }
}
