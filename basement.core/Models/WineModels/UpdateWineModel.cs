using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using basement.core.Entities.GeneralSection;

namespace basement.core.Models.WineModels
{
    public class UpdateWineModel
    {
        [Required] public long? WineId { get; set; }
        public string Name { get; set; }

        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }
        public string Producer { get; set; }

        public long DistrictId { get; set; }
        public double Alcohol { get; set; }
        //public ICollection<WineGrape> WineGrapes { get; set; }
    }
}