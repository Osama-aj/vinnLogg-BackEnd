using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace basement.core.Models.AddMetadata
{
    public class AddRegionModel
    {
        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required]
        public string RegionName { get; set; }


        [Required] public long CountryId { get; set; }
    }
}
