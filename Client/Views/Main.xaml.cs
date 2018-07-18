using System;
using System.Windows;
using NetLibrary.Models;
using Net = Client.Engine.Classes;
using NetLibrary.Enums;
using NetLibrary.EventsArgs;
using static Client.Engine.Classes.Client;

namespace Client.Views
{
    public partial class Main : Window
    {
        private Net.Client client;

        public Main()
        {
            InitializeComponent();

            client = new Net.Client();
        }
    }
}
