namespace basement.core.Entities.GeneralSection
{
    public class WineGrape
    {
        public long WineId { get; set; }
        public Wine Wine { get; set; }
        public double Percent { get; set; }
        public long GrapeId { get; set; }
        public Grape Grape { get; set; }
    }
}