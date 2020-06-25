using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace basement.core.Models.GradeModels
{
    public class AddGradeModel
    {
        [Required(ErrorMessage = "vintage id is required")]
        public long? VintageId { get; set; }

        [Range(1, 5)] public int Grade { get; set; }
        public string Comment { get; set; }
    }
}
