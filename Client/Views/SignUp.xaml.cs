using Client.Engine.Models;
using NetLibrary.Enums;
using NetLibrary.EventsArgs;
using Newtonsoft.Json.Linq;
using System;
using System.Windows;
using Net = Client.Engine.Classes;

namespace Client.Views
{
    public partial class SignUp : Window
    {
        public SignUpResults SignUpResult { get; set; } = SignUpResults.Undefined;

        public enum SignUpResults
        {
            Failed,
            Success,
            LinkToSignIn,
            Undefined
        }

        public SignUp()
        {
            InitializeComponent();

            CurrentConnection.CurrentClient.OnReceiveCommandResult -= OnReceiveCommandResult;
            CurrentConnection.CurrentClient.OnReceiveCommandResult += OnReceiveCommandResult;
        }

        private void OnReceiveCommandResult(object sender, ReceivedCommandResultsEventsArgs e)
        {
            if (e.StatusCode == StatusCodes.BadRequest)
            {
                Dispatcher.Invoke(() =>
                {
                    ErrorField.Visibility = Visibility.Visible;
                    ErrorField.Content = e.CommandResult["Message"].ToString();
                });
                return;
            }

            MessageBox.Show("Registration completed successfully", "Swype", MessageBoxButton.OK, MessageBoxImage.Information);

            SignUpResult = SignUpResults.Success;
            Close();
        }

        private void SignInBtn_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            SignUpResult = SignUpResults.LinkToSignIn;
            Close();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            CurrentConnection.CurrentClient.StopReceivingResponses();
            CurrentConnection.CurrentClient.OnReceiveCommandResult -= OnReceiveCommandResult;

            if (SignUpResult == SignUpResults.Undefined)
                Environment.Exit(0);
        }

        private async void SignUpBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearErrorField();

            var regData = new JObject
            {
                {"Email", EmailField.Text },
                {"Name", NameField.Text },
                {"Password", PasswordField.Password }
            };

            await CurrentConnection.CurrentClient.SendCommandAsync(CommandTypes.Registration, regData);

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
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                ErrorField.Visibility = Visibility.Visible;
                ErrorField.Content = ex.Message;
                return;
            }
        }
    }
}
