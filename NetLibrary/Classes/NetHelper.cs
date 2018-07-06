using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace NetLibrary.Classes
{
    public static class NetHelper
    {
        /// <summary>
        /// Get response data from server
        /// </summary>
        /// <param name="tcpClient">Current client connection</param>
        /// <returns>Response data</returns>
        public static Packet GetData(TcpClient tcpClient)
        {
            byte[] responseData = new byte[2048];

            int bytes = 0;

            do
            {
                bytes = tcpClient.GetStream().Read(responseData, 0, responseData.Length);
            }

            while (tcpClient.GetStream().DataAvailable);

            return FromByteArray<Packet>(responseData);
        }

        /// <summary>
        /// Send request data to server
        /// </summary>
        /// <param name="tcpClient">Current client connection</param>
        /// <param name="data">Data which need transfer to server</param>
        public static void SendData(TcpClient tcpClient, Packet data)
        {
            var bytes = ToByteArray<Packet>(data);
            tcpClient.GetStream().Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Get async response data from server
        /// </summary>
        /// <param name="tcpClient">Current client connection</param>
        /// <returns>Response data</returns>
        public static async Task<Packet> GetDataAsync(TcpClient tcpClient)
        {
            byte[] responseData = new byte[2048];

            int bytes = 0;

            do
            {
                bytes = await tcpClient.GetStream().ReadAsync(responseData, 0, responseData.Length);
            }

            while (tcpClient.GetStream().DataAvailable);

            return FromByteArray<Packet>(responseData);
        }

        /// <summary>
        /// Send async request data to server
        /// </summary>
        /// <param name="tcpClient">Current client connection</param>
        /// <param name="data">Data which need transfer to server</param>
        public static async void SendDataAsync(TcpClient tcpClient, Packet data)
        {
            var bytes = ToByteArray<Packet>(data);
            await tcpClient.GetStream().WriteAsync(bytes, 0, bytes.Length);
        }

        public static byte[] ToByteArray<T>(T obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ((MemoryStream)ms).ToArray();
            }
        }

        public static T FromByteArray<T>(byte[] data)
        {
            if (data == null)
                return default(T);
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(data))
            {
                object obj = bf.Deserialize(ms);
                return (T)obj;
            }
        }
    }
}
