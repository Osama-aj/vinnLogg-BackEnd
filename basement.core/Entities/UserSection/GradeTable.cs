using basement.core.Entities.GeneralSection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace basement.core.Entities.UserSection
{
    public class GradeTable
    {

        [Key] public long GradeTableId { get; set; }

        //0 is not set yet, and 1-5 is the stars
        [Range(0, 5)]
        public int Grade { get; set; } = 0;


        [Column(TypeName = "VARCHAR")]
        [StringLength(250)]
        public string Comment { get; set; } = "No Comment";


        [Required] public long VintageId { get; set; }


        [Required] public string UserId { get; set; }

    }
}
