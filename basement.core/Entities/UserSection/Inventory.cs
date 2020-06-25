using System.ComponentModel.DataAnnotations;
using basement.core.Entities.GeneralSection;

namespace basement.core.Entities.UserSection
{
    public class Inventory
    {
       
        [Key] public long InventoryId { get; set; }
        [Range(1, int.MaxValue,
           ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        [MaxLength(1), MinLength(int.MaxValue)] [Required] public int Amount { get; set; }
        [Required] public long VintageId { get; set; }
        public Vintage Vintage { get; set; }
        [Required] public long ShelfId { get; set; }
        public Shelf Shelf { get; set; }
    }
}