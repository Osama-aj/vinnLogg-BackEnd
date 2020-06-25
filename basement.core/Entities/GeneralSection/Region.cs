using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace basement.core.Entities.GeneralSection
{
    public class Region
    {
        [Key] public long RegionId { get; set; }
       
        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required] 
        public string RegionName { get; set; }
       
        
        [Required] public long CountryId { get; set; }
        public Country Country { get; set; }
        public virtual ICollection<District> Districts { get; set; }
    }
}