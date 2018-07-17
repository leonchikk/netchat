using Net = Client.Engine.Classes;
using System.Windows;
using Client.Views;
using System;

namespace Client
{
    public partial class App : Application
    {
        private Net.Client _client;

        public App()
        {
            //Приложение работает, пока не будет явно вызвано Application.Shutdown()
            this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            _client = new Net.Client();

            bool isUserAuthenticated = false;

            do
            {
                var signIn = new SignIn(ref _client);

                signIn.ShowDialog();

                switch (signIn.SignInResult)
                {
                    case SignIn.SignInResults.LinkToSignUp:
                        var signUp = new SignUp();
                        signUp.ShowDialog();

                        switch (signUp.SignUpResult)
                        {
                            case SignUp.SignUpResults.Undefined:
                                Environment.Exit(0);
                                break;

                            case SignUp.SignUpResults.LinkToSignIn:
                                continue;
                        }

                        break;
                }
            }
            while (!isUserAuthenticated);
        }
    }
}
