using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace NetLibrary.Helpers
{
    public static class ImageHelper
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapImage ToBitmapImage(this Bitmap bitmap)
        {
            var memory = new MemoryStream();

            bitmap.Save(memory, ImageFormat.Jpeg);
            memory.Position = 0;

            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();

            return bitmapImage;
        }

        public static BitmapImage ByteArrayToImage(Byte[] imageData)
        {
            MemoryStream ms = new MemoryStream(imageData);
            Image img = Image.FromStream(ms);
            //img.Save(imageName, System.Drawing.Imaging.ImageFormat.Jpeg);
            //videoBox.Image = img;
            //ms.Close()

            return null;
        }

        //public static BitmapImage ByteArrayToImage(Byte[] imageData)
        //{
        //    return null;
        //}

        //public static byte[] ImageToByteArray(BitmapImage bitmapImage)
        //{
        //    Stream stream = bitmapImage.StreamSource;
        //    Byte[] buffer = null;
        //    if (stream != null && stream.Length > 0)
        //    {
        //        using (BinaryReader br = new BinaryReader(stream))
        //        {
        //            buffer = br.ReadBytes((Int32)stream.Length);
        //        }
        //    }
        //    return buffer;
        //}
    }
}
