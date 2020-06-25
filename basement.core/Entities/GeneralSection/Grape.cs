using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace basement.core.Entities.GeneralSection
{
    public class Grape
    {
        [Key] public long GrapeId { get; set; }
      
        [Column(TypeName = "VARCHAR")]
        [StringLength(40)]
        [Required] public string GrapeName { get; set; }
       
        public ICollection<WineGrape> WineGrapes { get; set; }
    }
}