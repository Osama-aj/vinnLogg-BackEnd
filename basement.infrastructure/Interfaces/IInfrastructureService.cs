using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using basement.core.Entities.GeneralSection;
using basement.core.Models;
using basement.core.Models.AddMetadata;
using basement.core.Models.GeneralSectionDto;
using basement.core.Models.GradeModels;
using basement.core.Models.InventoryModels;
using basement.core.Models.ShelfModels;
using basement.core.Models.UserSectionDto;
using basement.core.Models.VintageModels;
using basement.core.Models.WineModels;

namespace basement.infrastructure.Interfaces
{
    public interface IInfrastructureService
    {
        //metadata
        public IList<CountryDto> GetAllCountriesRegionsDistricts();
        public IList<GrapeDto> GetAllGrapes();
        public MetaDataDto GetMetaData();


        public CountryDto AddCountry(AddCountryModel newCountry);
        public RegionDto AddRegion(AddRegionModel newRegion);
        public DistrictDto AddDistrict(AddDistrictModel newDistrict);

        //wine 

        public IEnumerable<WineDto> GetAllWinesOrBy(long wineId, long districtId, long regionId, long countryId,
            string startsWith, long grapeId);

        public WineDto AddWine(AddWineModel newWine);
        public WineDto UpdateWine(UpdateWineModel updateWine);
        public bool DeleteWine(long wineId);


        //vintage
        public IList<VintageDto> GetAllVintagesByWineId(long wineId);
        public VintageDto AddVintage(AddVintageModel newVintage);
        public VintageDto UpdateVintage(UpdateVintageModel updateVintage);
        public bool DeleteVintage(long vintageId);


        //shelf
        public IList<ShelfDto> GetAllShelves(ClaimsPrincipal user);
        public ShelfDto AddShelf(AddShelfModel newShelve, ClaimsPrincipal user);
        public ShelfDto UpdateShelf(UpdateShelfModel updateShelf, ClaimsPrincipal user);
        public bool DeleteShelf(long shelfId, ClaimsPrincipal user);


        //inventory
        public InventoryDto AddInventory(AddInventoryModel newInventory, ClaimsPrincipal user);
        public InventoryDto UpdateInventory(UpdateInventoryModel updateInventory, ClaimsPrincipal user);
        public bool DeleteInventory(long inventoryId, ClaimsPrincipal user);

        //wine list
        public IList<WineListDto> GetUsersWineList(
            ClaimsPrincipal user,
            long shelfId = -1,
            long wineId = -1,
            long districtId = -1,
            long regionId = -1,
            long countryId = -1,
            string startsWith = "",
            long grapeId = -1,
            bool containsInstead = false);

        public IList<WineListDto> GetAllWineListWithUserInfo(
            ClaimsPrincipal user,
            long wineId = -1,
            long districtId = -1,
            long regionId = -1,
            long countryId = -1,
            string startsWith = "",
            long grapeId = -1,
            bool containsInstead = false
        );


        //Grades
        public GradeDto AddGrade(AddGradeModel AddGrade, ClaimsPrincipal user);
        public GradeDto UpdateGrade(UpdateGradeModel UpdateGrade, ClaimsPrincipal user);


        //images
        public byte[] GetBigImage(long wineId);
        public byte[] GetThumbnailImage(long wineId);


        public void Mock();
    }
}