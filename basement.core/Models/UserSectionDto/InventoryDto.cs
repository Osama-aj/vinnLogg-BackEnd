using basement.core.Models.GeneralSectionDto;

namespace basement.core.Models.UserSectionDto
{
    public class InventoryDto
    {
        public long InventoryId { get; set; }
        public int Amount { get; set; }
        public long ShelfId { get; set; }

        public long VintageId { get; set; }
        public VintageDto Vintage { get; set; }
    }
}