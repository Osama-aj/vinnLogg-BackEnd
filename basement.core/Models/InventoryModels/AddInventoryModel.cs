using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.InventoryModels
{
    public class AddInventoryModel
    {
        [Range(1, int.MaxValue,
           ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [Required] public int? Amount { get; set; }
        [Required] public long? VintageId { get; set; }
        [Required] public long? ShelfId { get; set; }
    }
}