using Castle.MicroKernel.SubSystems.Conversion;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace basement.core.Entities.GeneralSection
{
    public class Country
    {
        [Key] public long CountryId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required] public string CountryName { get; set; }
        public virtual ICollection<Region> Regions { get; set; }
    }
}