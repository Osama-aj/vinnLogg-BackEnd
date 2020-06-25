using basement.core.Entities.UserSection;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace basement.core.Entities.GeneralSection
{
    public class Vintage
    {
        [Key] public long VintageId { get; set; }
        
        
        [Required]
        public int Year { get; set; }
       
        
        public string EAN { get; set; }
        [Required] public long WineId { get; set; }
        public Wine Wine { get; set; }
        public GradeTable Grade { get; set; }

    }
}