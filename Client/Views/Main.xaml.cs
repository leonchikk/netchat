using Client.Engine.Models;
using NetLibrary.Enums;
using Newtonsoft.Json.Linq;
using NetLibrary.EventsArgs;
using NetLibrary.Models;
using System.Windows;
using System;

namespace Client.Views
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
            CurrentConnection.CurrentClient.OnReceiveCommandResult += OnReceiveCommandResult;
        }

        private void OnReceiveCommandResult(object sender, ReceivedCommandResultsEventsArgs e)
        {
            switch (e.CommandType)
            {
                case CommandTypes.UserInfo:

                    if (e.StatusCode == StatusCodes.BadRequest)
                        return;

                    CurrentConnection.CurrentClientInfo = new ClientModel
                    {
                        //Need add auto changing status
                        ClientState = UserStates.Online,
                        Id = Guid.Parse(e.CommandResult["UserId"].ToString()),
                        Email = e.CommandResult["UserEmail"].ToString(),
                        Name = e.CommandResult["UserName"].ToString()
                    };

                    UserNameField.DataContext = CurrentConnection.CurrentClientInfo;

                    break;
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.UserInfo, new JObject { { "Token", CurrentConnection.ClientToken } });
        }
    }
}
