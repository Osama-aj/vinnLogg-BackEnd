using System.ComponentModel.DataAnnotations;

namespace basement.core.Models.InventoryModels
{
    public class UpdateInventoryModel
    {
        [Required] public long? InventoryId { get; set; }
        [Range(0, int.MaxValue,
         ErrorMessage = "Value for {0} must be between {1} and {2}.")]

        public int? Amount { get; set; }
        public long? ShelfId { get; set; }

    }
}