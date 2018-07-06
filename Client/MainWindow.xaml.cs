using System;
using System.Windows;
using static NetLibrary.Classes.Client;
using NetLibrary.Models;
using Net = NetLibrary.Classes;
using NetLibrary.EventsArgs;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Net.Client client;

        public MainWindow()
        {
            InitializeComponent();
            string error;

            client = new Net.Client();

            client.OnConnected += new ConnectionHandler(delegate (object o, EventArgs a)
            {
                Net.Packet info = new Net.Packet
                {
                    ActionState = Net.ActionState.Connect,
                    ClientInfo = new ClientModel
                    {
                        ClientState = Net.UserState.Online,
                        Id = Guid.NewGuid(),
                        Ip = "127.0.0.1",
                        Login = "Leo",
                        Name = "Elijah"
                    }
                };

                client.SendRequestAsync(info, out error);
            });

            client.OnDisconnected += new ConnectionHandler(delegate (object o, EventArgs a)
            {

            });

            client.OnReceivedResponse += new ReceiveResponseHandler(delegate (object sender, ReceiveResponseArgs e)
            {

            });
            
            client.ConnectToServer("127.0.0.1", 27015, out error);

        }
    }
}
