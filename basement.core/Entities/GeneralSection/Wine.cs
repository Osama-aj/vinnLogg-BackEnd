using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace basement.core.Entities.GeneralSection
{
    public class Wine
    {
        [Key] public long WineId { get; set; }
        [Required] public string Name { get; set; }
        public string Producer { get; set; }

        [Required] public long DistrictId { get; set; }
        public District District { get; set; }

        public double Alcohol { get; set; }
        public ICollection<WineGrape> WineGrapes { get; set; }
        public ICollection<Vintage> Vintages { get; set; }
    }
}