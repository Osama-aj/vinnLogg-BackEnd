using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.ShelfModels
{
    public class AddShelfModel
    {
        [StringLength(18)]
        [Required] public string ShelfName { get; set; }
    }
}