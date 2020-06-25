namespace basement.core.Models.GeneralSectionDto
{
    public class VintageDto
    {
        public long VintageId { get; set; }
        public int Year { get; set; }
        public string EAN { get; set; }

        public long WineId { get; set; }
        public WineDto Wine { get; set; }
    }
}