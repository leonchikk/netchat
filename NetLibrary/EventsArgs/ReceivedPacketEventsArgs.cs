using NetLibrary.Classes;

namespace NetLibrary.EventsArgs
{
    /// <summary>
    /// Event args for OnReceivedResponse event
    /// </summary>
    public class ReceivedPacketEventsArgs
    {
        private Packet _receivedPacket = null;

        public Packet ReceivedPacket
        {
            get { return _receivedPacket; }
        }

        public ReceivedPacketEventsArgs(Packet receivedPacket)
        {
            _receivedPacket = receivedPacket;
        }

    }
}
