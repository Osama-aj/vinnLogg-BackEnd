using System.Collections;
using System.Collections.Generic;

namespace basement.core.Models.GeneralSectionDto
{
    public class MetaDataDto
    {
        public IEnumerable<GrapeDto> Grapes { get; set; }
        public IEnumerable<CountryDto> Countries { get; set; }
    }
}