using Net = Client.Engine.Classes;
using NetLibrary.EventsArgs;
using NetLibrary.Enums;
using System.Windows;
using System;
using Newtonsoft.Json.Linq;
using Client.Engine.Models;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for SignIn.xaml
    /// </summary>
    public partial class SignIn : Window
    {
        public SignInResults SignInResult { get; set; } = SignInResults.Undefined;

        public enum SignInResults
        {
            Failed,
            Success,
            LinkToSignUp,
            Undefined
        }

        public SignIn()
        {
            InitializeComponent();
            CurrentConnection.CurrentClient.OnReceiveCommandResult += OnReceiveCommandResult;
        }

        private void OnReceiveCommandResult(object sender, ReceivedCommandResultsEventsArgs e)
        {
            if (e.StatusCode == StatusCodes.BadRequest)
            {
                ErrorField.Visibility = Visibility.Visible;
                ErrorField.Content = e.CommandResult["Message"].ToString();
                return;
            }
        
            CurrentConnection.ClientToken = e.CommandResult["Token"].ToString();

            SignInResult = SignInResults.Success;
            Close();
        }

        private void SignUpBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SignInResult = SignInResults.LinkToSignUp;
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            CurrentConnection.CurrentClient.StopReceivingResponses();
            CurrentConnection.CurrentClient.OnReceiveCommandResult -= OnReceiveCommandResult;

            if (SignInResult == SignInResults.Undefined)
                Environment.Exit(0);
        }

        private async void SignInBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearErrorField();

            var authData = new JObject
            {
                {"Email", EmailField.Text },
                {"Password", PasswordField.Password }
            };

            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.Authorization, authData);
        }

        private void ClearErrorField()
        {
            ErrorField.Visibility = Visibility.Hidden;
            ErrorField.Content = string.Empty;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await CurrentConnection.CurrentClient.StartReceivingResponsesAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                ErrorField.Visibility = Visibility.Visible;
                ErrorField.Content = ex.Message;
                return;
            }
        }
    }
}
