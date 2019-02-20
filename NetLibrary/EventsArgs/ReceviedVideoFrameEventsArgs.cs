using NetLibrary.Models;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace NetLibrary.EventsArgs
{
    public class ReceviedVideoFrameEventsArgs
    {
        public ClientModel Sender { get; set; }
        public BitmapImage Frame { get; set; }

        public ReceviedVideoFrameEventsArgs(ClientModel sender, BitmapImage frame)
        {
            Sender = sender;
            Frame = frame;
        }
    }
}
