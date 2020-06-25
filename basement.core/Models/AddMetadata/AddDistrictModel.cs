using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace basement.core.Models.AddMetadata
{
    public class AddDistrictModel
    {
        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required]
        public string DistrictName { get; set; }
        // [ForeignKey("Region")]
        [Required] public long RegionId { get; set; }
    }
}
