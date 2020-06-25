using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using basement.core.Entities.GeneralSection;

namespace basement.core.Models.WineModels
{
    public class AddWineModel
    {
        [Required] public string Name { get; set; }
        [JsonConverter(typeof(Base64FileJsonConverter))]
        public byte[] Image { get; set; }
        [Required] public long? DistrictId { get; set; }
         public string Producer { get; set; }

        public double Alcohol { get; set; }
         public ICollection<WineGrape> WineGrapes { get; set; }

    }
}