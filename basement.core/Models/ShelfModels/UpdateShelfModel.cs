using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.ShelfModels
{
    public class UpdateShelfModel
    {
        [Required] public long? ShelfId { get; set; }
        [StringLength(18)]
        [Required] public string ShelfName { get; set; }
    }
}