using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.VintageModels
{
    public class AddVintageModel
    {
        
        [Required]
        public int? Year { get; set; }
        public string EAN { get; set; }
        [Required] public long? WineId { get; set; }
    }
}