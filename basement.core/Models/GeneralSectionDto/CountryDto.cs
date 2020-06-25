using System.Collections.Generic;

namespace basement.core.Models.GeneralSectionDto
{
    public class CountryDto
    {
        public long CountryId { get; set; }
        public string CountryName { get; set; }
        public virtual IList<RegionDto> Regions { get; set; }
    }
}