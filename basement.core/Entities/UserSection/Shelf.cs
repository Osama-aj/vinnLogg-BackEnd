using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace basement.core.Entities.UserSection
{
    public class Shelf
    {
        [Key] public long ShelfId { get; set; }
        [StringLength(18)]
        [Required] public string ShelfName { get; set; }
        [Required] public string UserId { get; set; }

       // public ICollection<Inventory> Inventories { get; set; }
    }
}