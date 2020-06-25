using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace basement.core.Models.AddMetadata
{
    public class AddCountryModel
    {
        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required] public string CountryName { get; set; }
    }
}
