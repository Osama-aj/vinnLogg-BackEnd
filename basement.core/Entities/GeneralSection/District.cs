using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace basement.core.Entities.GeneralSection
{
    public class District
    {
        [Key] public long DistrictId { get; set; }


        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required]
        public string DistrictName { get; set; }
       // [ForeignKey("Region")]
        [Required] public long RegionId { get; set; }
        [Required] public Region Region { get; set; }
    }
}