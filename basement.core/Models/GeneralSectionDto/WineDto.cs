using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace basement.core.Models.GeneralSectionDto
{
    public class WineDto
    {
        public long WineId { get; set; }
        public string Name { get; set; }
        public string ImageBig { get; set; }
        public string ImageThumbnail { get; set; }
        public string Producer { get; set; }

        public double Alcohol { get; set; }
        public DistrictDto District { get; set; }
        public ICollection<WineGrapeDto> WineGrapes { get; set; }
    }
}