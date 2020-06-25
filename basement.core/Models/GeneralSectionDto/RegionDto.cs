using System.Collections.Generic;

namespace basement.core.Models.GeneralSectionDto
{
    public class RegionDto
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; }
        public long CountryId { get; set; }
        public virtual IList<DistrictDto> Districts { get; set; }
    }
}