using System.Collections.Generic;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using basement.core.Entities.GeneralSection;
using basement.core.Entities.UserSection;
using basement.core.Models.UserModels;
using basement.core.Models.UserSectionDto;

namespace basement.infrastructure.Interfaces
{
    public interface IDatabaseBoundary
    {
        //metadata
        public IList<Country> GetAllCountriesRegionsDistricts();
        public IEnumerable<Grape> GetAllGrapes();

        public Country AddCountry(Country country);
        public Region AddRegion(Region region);
        public District AddDistrict(District district);
        // wine  

        public IList<Wine> GetAllWinesOrBy(long wineId = -1, long districtId = -1, long regionId = -1,
            long countryId = -1, string startsWith = "", long grapeId = -1, bool contains = false);

        public Wine GetWineById(long wineId);
        public bool CheckIfWineExistByName(string wineName);

        public Wine AddWine(Wine newWine);
        public bool UpdateWine(Wine updatedWine);
        public bool DeleteWine(Wine wineToRemove);


        //vintage   
        public IList<Vintage> GetAllVintagesByWineId(long wineId);
        public Vintage GetVintageById(long vintageId);
        public IList<Vintage> GetAllVintages(IList<long> wineIds);

        public Vintage AddVintage(Vintage newVintages);
        public bool UpdateVintage(Vintage updatedVintage);
        public bool DeleteVintage(Vintage vintageToDelete);


        //user 
        // public User GetUserByUserName(string username);
        public User GetUserById(string id);
        public bool AddUser(User user);
        public bool UpdateUser(User user);
        public bool DeleteUser(User user);


        //shelf
        public IList<Shelf> GetAllShelves(string userId);
        public Shelf GetShelfById(long shelfId);
        public Shelf AddShelf(Shelf newShelve);
        public bool UpdateShelf(Shelf updateShelf);
        public bool DeleteShelf(Shelf shelf);


        //Inventory

        public IList<Inventory> GetUsersInventory(string userId);
        public Inventory GetInventoryById(long inventoryId);
        public bool AddInventory(Inventory newInventories);
        public bool UpdateInventory(Inventory updateInventory);
        public bool DeleteInventory(Inventory inventory);


        // wine list

        public IList<Inventory> GetUsersWineList(
            string userId,
            long shelfId = -1,
            long wineId = -1,
            long districtId = -1,
            long regionId = -1,
            long countryId = -1,
            string startsWith = "",
            long grapeId = -1,
            bool contains = false);

        // public IList<Inventory> GetAllWinesExcludeUser(
        //     String userId,
        //     long wineId = -1,
        //     long districtId = -1,
        //     long regionId = -1,
        //     long countryId = -1,
        //     string startsWith = "",
        //     long grapeId = -1);


        // grades

        public GradeTable GetGradeById(long gradeId);
        public IList<GradeTable> GetGradesByUserId(string userId);
        public GradeTable GetGradeByUserIdAndVintageId(string userId, long vintageId);
        public bool AddGrade(GradeTable grade);
        public bool UpdateGrade(GradeTable grade);

        // mock data
        public void Mock();
    }
}