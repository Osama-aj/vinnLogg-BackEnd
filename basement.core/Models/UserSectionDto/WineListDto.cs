using System.Collections.Generic;
using basement.core.Entities.UserSection;
using basement.core.Models.GeneralSectionDto;
using basement.core.Models.GradeModels;

namespace basement.core.Models.UserSectionDto
{
    public class WineListDto
    {
        public long WineId { get; set; }
        public string WineName { get; set; }
        public string ImageBig { get; set; }
        public string ImageThumbnail { get; set; }
        public string Producer { get; set; }
        public double Alcohol { get; set; }
        public DistrictDto District { get; set; }
        public WinelistRegionDto Region { get; set; }
        public WinelistCountryDto Country { get; set; }
        
        public ICollection<WineGrapeDto> WineGrapes { get; set; }



        public ICollection<WineListVintagesDtio> Vintages { get; set; }
    }



    public class WineListVintagesDtio
    {
        public long WineId { get; set; }


        public long VintageId { get; set; }
        public int Year { get; set; }
        public string EAN { get; set; }



        public long InventoryId { get; set; }
        public int Amount { get; set; }
        public long ShelfId { get; set; }
        public string ShelfName { get; set; }


        public GradeDto Grade { get; set; }
        //public WineListInventoryDto Inventory { get; set; }

    }

    public class WinelistRegionDto
    {
        public long RegionId { get; set; }
        public string RegionName { get; set; }
        public long CountryId { get; set; }
    }

    public class WinelistCountryDto
    {
        public long CountryId { get; set; }
        public string CountryName { get; set; }
    }
    //public class WineListInventoryDto
    //{
    //    public long InventoryId { get; set; }
    //    public int Amount { get; set; }
    //    public long ShelfId { get; set; }
    //    public string ShelfName { get; set; }
    //}



}