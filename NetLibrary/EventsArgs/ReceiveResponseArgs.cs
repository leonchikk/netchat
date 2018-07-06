using NetLibrary.Classes;

namespace NetLibrary.EventsArgs
{
    /// <summary>
    /// Event args for OnReceivedResponse event
    /// </summary>
    public class ReceiveResponseArgs
    {
        private Packet _receivedPacket = null;

        public Packet ReceivedPacket
        {
            get { return _receivedPacket; }
        }

        public ReceiveResponseArgs(Packet receivedPacket)
        {
            _receivedPacket = receivedPacket;
        }

    }
}
