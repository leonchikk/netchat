using System.Windows;
using Net = NetLibrary.Classes;

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

            Net.Server server = new Net.Server();
            server.Start("127.0.0.1", 27015, out error);
        }
    }
}
