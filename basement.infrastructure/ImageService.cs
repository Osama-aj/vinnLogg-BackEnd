using basement.core.Models;
using basement.infrastructure.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace basement.infrastructure
{
    public class ImageService : IImageService
    {
        public static char separator = Path.DirectorySeparatorChar;
        public static string bigImagePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages" + separator + "big" + separator + "/wineId" + ".jpg";
        public static string thumbnailImagePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages" + separator + "thumbnail" + separator + "/wineId" + ".png";

        public byte[] GetBigImage(long wineId)
        {
            var imgPath  = bigImagePath.Replace("/wineId", wineId.ToString());
            var isExist = System.IO.File.Exists(imgPath);
            if (!isExist)
                return null;
            try
            {
                var imgData = System.IO.File.ReadAllBytes(imgPath);
                return imgData;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }


        }

        public byte[] GetThumbnailImage(long wineId)
        {
            var imgPath = thumbnailImagePath.Replace("/wineId", wineId.ToString());
            var isExist = System.IO.File.Exists(imgPath);
            if (!isExist)
                return null;
            try
            {
                var imgData = System.IO.File.ReadAllBytes(imgPath);
                return imgData;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }

    }
}
