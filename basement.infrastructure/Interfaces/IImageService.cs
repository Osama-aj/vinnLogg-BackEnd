using basement.core.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace basement.infrastructure.Interfaces
{
    public interface IImageService
    {

        public byte[] GetBigImage(long wineId);
        public byte[] GetThumbnailImage(long wineId);

    }
}
