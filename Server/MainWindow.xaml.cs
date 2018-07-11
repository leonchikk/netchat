using System.Windows;
using Engine = Server.Engine.Classes;

namespace Server
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            string error;

            Engine.Classes.Server server = new Engine.Classes.Server();
            server.Start("127.0.0.1", 27015, out error);
        }
    }
}
