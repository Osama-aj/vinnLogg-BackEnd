using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using System.Drawing.Imaging;
using System.Drawing;
using Image = System.Drawing.Image;

namespace basement.infrastructure.Helpers
{
    public static class IncomigImageService
    {
        public static char separator = Path.DirectorySeparatorChar;
        public static string imageFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages";


       

        public static bool saveImageToDisk(byte[] img, long wineId)
        {
            var isSucceed = true;
            var imgOriginalName = imageFolderPath + separator + "original" + separator + wineId.ToString() + ".jpg";
            var imgBigName = imageFolderPath + separator + "big" + separator + wineId.ToString() + ".jpg";
            var imgthumbnailName = imageFolderPath + separator + "thumbnail" + separator + wineId.ToString() + ".png";



            try
            {
                //write original image to disk
                System.IO.File.WriteAllBytes(imgOriginalName, img);

                //write thumbnail image to disk
                var thumbnail = img.ResizeImage(100,ImageFormat.Png);
                System.IO.File.WriteAllBytes(imgthumbnailName, thumbnail);

                //write big image to disk
                var big = img.ResizeImage(250,ImageFormat.Jpeg);
                System.IO.File.WriteAllBytes(imgBigName, big);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                isSucceed = false;
            }


            return isSucceed;
        }






        //TODO: move these two methods to IImageService
        public static string GetOriginalImageUrl(long wineId)
        {
            var imagePath = imageFolderPath + separator + "original" + separator + wineId.ToString() + ".jpg";
            var isExist = System.IO.File.Exists(imagePath);
            var imgOriginalName = isExist ? imagePath : null;
            var imageUrl = "http://54.72.51.80:5000/api/images/big?wineid=";
            imageUrl += isExist ? wineId.ToString() : "-1";
            return imageUrl;
        }

        public static string GetThumbnailImageUrl(long wineId)
        {
            var imagePath = imageFolderPath + separator + "thumbnail" + separator + wineId.ToString() + ".png";
            var isExist = System.IO.File.Exists(imagePath);
            var imgthumbnailName = isExist ? imagePath : null;
            var imageUrl = "http://54.72.51.80:5000/api/images/thumbnail?wineid=";
            imageUrl += isExist ? wineId.ToString() : "-1";
            return imageUrl;
        }







        //////////////////////
        //////////////////////
        //////////////////////
        //////////////////////
        /// helper methods "private"
        private static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                ms.Close();
                byteArray = ms.ToArray();
            }
            return byteArray;
        }

        private static Image ToImage(this byte[] byteArrayIn)
        {

            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                return Image.FromStream(ms);
            }

        }

     
        private  static byte[] ResizeImage(this byte[] data, int width,ImageFormat format)
        {
            using (var stream = new MemoryStream(data))
            {
                var image = Image.FromStream(stream);

                var height = (width * image.Height) / image.Width;
                var thumbnail = image.GetThumbnailImage(width, height, null, IntPtr.Zero);

                using (var thumbnailStream = new MemoryStream())
                {
                    thumbnail.Save(thumbnailStream, format);
                    return thumbnailStream.ToArray();
                }
            }
        }

    }
}
