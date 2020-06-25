using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.VintageModels
{
    public class UpdateVintageModel
    {
        [Required] public long? VintageId { get; set; }

        public int Year { get; set; }
        public string EAN { get; set; }

    }
}