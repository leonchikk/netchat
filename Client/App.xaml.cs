using Net = Client.Engine.Classes;
using System.Windows;
using Client.Views;
using System;
using Client.Engine.Models;

namespace Client
{
    public partial class App : Application
    {

        public App()
        {
            //Приложение работает, по ка не будет явно вызвано Application.Shutdown()
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                await CurrentConnection.CurrentClient.TryConnectToServerAsync("127.0.0.1", 27015);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

            bool isUserAuthenticated = false;

            do
            {
                var signIn = new SignIn();

                signIn.ShowDialog();

                switch (signIn.SignInResult)
                {
                    case SignIn.SignInResults.Success:
                        isUserAuthenticated = true;
                        break;

                    case SignIn.SignInResults.LinkToSignUp:
                        var signUp = new SignUp();
                        signUp.ShowDialog();

                        switch (signUp.SignUpResult)
                        {
                            case SignUp.SignUpResults.Success:
                            case SignUp.SignUpResults.LinkToSignIn:
                                continue;
                        }

                        break;
                }
            }
            while (!isUserAuthenticated);

            var mainWindow = new Main();
            mainWindow.ShowDialog();

            Environment.Exit(0);
        }
    }
}
