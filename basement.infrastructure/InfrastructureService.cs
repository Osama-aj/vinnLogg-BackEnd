using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using basement.core.Entities.GeneralSection;
using basement.core.Entities.UserSection;
using basement.core.Models;
using basement.core.Models.GeneralSectionDto;
using basement.core.Models.InventoryModels;
using basement.core.Models.ShelfModels;
using basement.core.Models.UserSectionDto;
using basement.core.Models.VintageModels;
using basement.core.Models.WineModels;
using basement.infrastructure.Interfaces;
using basement.core.Models.GradeModels;
using System.Drawing;
using System.Drawing.Drawing2D;
using basement.infrastructure.Helpers;
using Region = basement.core.Entities.GeneralSection.Region;
using System.Collections.Immutable;
using basement.core.Models.AddMetadata;
using System.Data;

namespace basement.infrastructure
{
    public class InfrastructureService : IInfrastructureService
    {
        private readonly IDatabaseBoundary _databaseBoundary;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;


        public InfrastructureService(IDatabaseBoundary databaseBoundary, IMapper mapper, IUserService userService, IImageService imageService)
        {
            _databaseBoundary = databaseBoundary;
            _mapper = mapper;
            _userService = userService;
            _imageService = imageService;
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// metadata
        public IList<CountryDto> GetAllCountriesRegionsDistricts()
        {
            var countries = _databaseBoundary.GetAllCountriesRegionsDistricts();
            IList<CountryDto> countriesDto = _mapper.Map<Country[], IList<CountryDto>>(countries.ToArray());


            //sorting 
            countriesDto = countriesDto.OrderBy(c => c.CountryName).ToList();
            for (int i = 0; i < countriesDto.Count(); i++)
                if (countriesDto[i].Regions != null)
                {
                    countriesDto[i].Regions = countriesDto[i].Regions.OrderBy(r => r.RegionName).ToList();
                    for (int j = 0; j < countriesDto[i].Regions.Count(); j++)
                        if (countriesDto[i].Regions[j].Districts != null)
                            countriesDto[i].Regions[j].Districts = countriesDto[i].Regions[j].Districts.OrderBy(d => d.DistrictName).ToList();
                }

            return countriesDto;
        }

        public IList<GrapeDto> GetAllGrapes()
        {
            var grapes = _databaseBoundary.GetAllGrapes();
            IList<GrapeDto> grapesDto = _mapper.Map<Grape[], IList<GrapeDto>>(grapes.ToArray());

            return grapesDto.OrderBy(g => g.GrapeName).ToList();
        }

        public MetaDataDto GetMetaData()
        {
            MetaDataDto metaData = new MetaDataDto()
            {
                Grapes = GetAllGrapes(),
                Countries = GetAllCountriesRegionsDistricts()
            };
            return metaData;
        }
        ////////////////////////////////
        ////////////////////////////////
        ///  metadata post


        public CountryDto AddCountry(AddCountryModel newCountry)
        {
            var allCountries = GetAllCountriesRegionsDistricts();
            if (allCountries.Any(c => string.Equals(c.CountryName, newCountry.CountryName, StringComparison.OrdinalIgnoreCase)))
                return null;

            var countryToAdd = _mapper.Map<AddCountryModel, Country>(newCountry);

            var addedCountry = _databaseBoundary.AddCountry(countryToAdd);
            var countryDto = _mapper.Map<Country, CountryDto>(addedCountry);
            return countryDto;

        }

        public RegionDto AddRegion(AddRegionModel newRegion)
        {
            var allCountries = GetAllCountriesRegionsDistricts();
            var selectedCountry = allCountries.FirstOrDefault(c => c.CountryId == newRegion.CountryId);
            if (selectedCountry == null)
                return null;

            if (selectedCountry.Regions.Any(c => string.Equals(c.RegionName, newRegion.RegionName, StringComparison.OrdinalIgnoreCase)))
                return null;



            var regionToAdd = _mapper.Map<AddRegionModel, Region>(newRegion);

            var addedRegion = _databaseBoundary.AddRegion(regionToAdd);
            var regionDto = _mapper.Map<Region, RegionDto>(addedRegion);
            return regionDto;
        }

        public DistrictDto AddDistrict(AddDistrictModel newDistrict)
        {
            var allCountries = GetAllCountriesRegionsDistricts();
            CountryDto selectedCountry = null;
            bool found = false;
            foreach (var country in allCountries)
            {
                foreach (var region in country.Regions)
                {
                    if (region.RegionId == newDistrict.RegionId)
                    {
                        selectedCountry = country;
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            if (selectedCountry == null)
                return null;



            var selectedRegion = selectedCountry.Regions.FirstOrDefault(r => r.RegionId == newDistrict.RegionId);
            if (selectedRegion == null)
                return null;

            if (selectedRegion.Districts.Any(d => string.Equals(d.DistrictName, newDistrict.DistrictName, StringComparison.OrdinalIgnoreCase)))
                return null;



            var districtToAdd = _mapper.Map<AddDistrictModel, District>(newDistrict);

            var addedDistrict = _databaseBoundary.AddDistrict(districtToAdd);
            var districtDto = _mapper.Map<District, DistrictDto>(addedDistrict);
            return districtDto;
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ///  wine
        public IEnumerable<WineDto> GetAllWinesOrBy(long wineId, long districtId, long regionId, long countryId,
            string startsWith, long grapeId)
        {
            var wines = _databaseBoundary.GetAllWinesOrBy(wineId, districtId, regionId, countryId, startsWith, grapeId);
            IList<WineDto> winesDto = _mapper.Map<Wine[], IList<WineDto>>(wines.ToArray());


            foreach (var wine in winesDto)
            {
                wine.ImageBig = IncomigImageService.GetOriginalImageUrl(wine.WineId);
                wine.ImageThumbnail = IncomigImageService.GetThumbnailImageUrl(wine.WineId);
            }
            return winesDto.OrderBy(w => w.Name).ToList();
        }


        public WineDto AddWine(AddWineModel newWine)
        {
            var isExist = _databaseBoundary.CheckIfWineExistByName(newWine.Name);
            if (isExist)
            {
                return null;
            }

            newWine.Name = FirstLetterToUpperCase(newWine.Name.ToLower());
            Wine wine = _mapper.Map<AddWineModel, Wine>(newWine);
            var addedWine = _databaseBoundary.AddWine(wine);
            if (addedWine == null)
                return null;

            //do something if the image have not been saved to the disk, 
            //like notify the user to edit the winr with the right image
            var isSuccess = IncomigImageService.saveImageToDisk(newWine.Image, addedWine.WineId);

            WineDto wineDto = _mapper.Map<Wine, WineDto>(addedWine);

            wineDto.ImageBig = IncomigImageService.GetOriginalImageUrl(wineDto.WineId);
            wineDto.ImageThumbnail = IncomigImageService.GetThumbnailImageUrl(wineDto.WineId);
            return wineDto;
        }

        public WineDto UpdateWine(UpdateWineModel updateWine)
        {
            var wineToUpdate = _databaseBoundary.GetWineById((long)updateWine.WineId);
            if (wineToUpdate == null)
                return null;


            _mapper.Map(updateWine, wineToUpdate);
            wineToUpdate.District = null;

            var isSuccess = _databaseBoundary.UpdateWine(wineToUpdate);
            if (!isSuccess)
                return null;
            if (updateWine.Image != null)
            {
                var saveImageSuccess = IncomigImageService.saveImageToDisk(updateWine.Image, wineToUpdate.WineId);
            }

            var resultWine = _databaseBoundary.GetWineById((long)updateWine.WineId);
            var wineDto = _mapper.Map<Wine, WineDto>(resultWine);


            wineDto.ImageBig = IncomigImageService.GetOriginalImageUrl(wineDto.WineId);
            wineDto.ImageThumbnail = IncomigImageService.GetThumbnailImageUrl(wineDto.WineId);
            return wineDto;
        }


        public bool DeleteWine(long wineId)
        {
            var wineToRemove = _databaseBoundary.GetWineById(wineId);
            if (wineToRemove == null)
                return false;

            var isSuccess = _databaseBoundary.DeleteWine(wineToRemove);

            return isSuccess;
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ///  shelf
        public IList<ShelfDto> GetAllShelves(ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;
            var shelves = _databaseBoundary.GetAllShelves(userId);
            var shelvesDto = _mapper.Map<Shelf[], List<ShelfDto>>(shelves.ToArray());

            return shelvesDto.OrderBy(s => s.Name, new CustomShelfnameComparer()).ToList();
        }

        public ShelfDto AddShelf(AddShelfModel newShelve, ClaimsPrincipal user)
        {
            var shelfToAdd = _mapper.Map<AddShelfModel, Shelf>(newShelve);

            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;
            shelfToAdd.UserId = userId;

            var usersShelves = _databaseBoundary.GetAllShelves(userId);
            if (usersShelves.Any(s => string.Equals(s.ShelfName, newShelve.ShelfName, StringComparison.OrdinalIgnoreCase)))
                return null;
            var addedShelves = _databaseBoundary.AddShelf(shelfToAdd);
            var shelvesDto = _mapper.Map<Shelf, ShelfDto>(addedShelves);

            return shelvesDto;
        }

        public ShelfDto UpdateShelf(UpdateShelfModel updateShelf, ClaimsPrincipal user)
        {
            var shelfToUpdate = _databaseBoundary.GetShelfById((long)updateShelf.ShelfId);
            if (shelfToUpdate == null)
                throw new NullReferenceException("Shelf not found");

            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;
            if (shelfToUpdate.UserId != userId)
                return null;

            _mapper.Map(updateShelf, shelfToUpdate);

            var isSuccess = _databaseBoundary.UpdateShelf(shelfToUpdate);
            if (!isSuccess)
                return null;


            var shelfDto = _mapper.Map<Shelf, ShelfDto>(shelfToUpdate);

            return shelfDto;
        }

        public bool DeleteShelf(long shelfId, ClaimsPrincipal user)
        {
            var shelfToRemove = _databaseBoundary.GetShelfById(shelfId);
            if (shelfToRemove == null)
                throw new NullReferenceException("Shelf not found");


            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return false;
            if (shelfToRemove.UserId != userId)
                return false;

            var isSuccess = _databaseBoundary.DeleteShelf(shelfToRemove);

            return isSuccess;
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ///  inventory
        public InventoryDto AddInventory(AddInventoryModel newInventory, ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;
            var shelf = _databaseBoundary.GetShelfById((long)newInventory.ShelfId);
            if (shelf == null)
                throw new NullReferenceException("Shelf not found");
            if (shelf.UserId != userId)
                return null;

            var usersInventories = _databaseBoundary.GetUsersInventory(userId);
            var inventoriesExist = usersInventories.Any(ui => ui.VintageId == newInventory.VintageId && ui.ShelfId == newInventory.ShelfId);

            if (inventoriesExist)
            {
                var invToUpdate = usersInventories.First(ui => ui.VintageId == newInventory.VintageId && ui.ShelfId == newInventory.ShelfId);
                invToUpdate.Amount += (int)newInventory.Amount;
                var updatedSuccessfully = _databaseBoundary.UpdateInventory(invToUpdate);
                if (updatedSuccessfully)
                {
                    var updatedInventoriesDto = _mapper.Map<Inventory, InventoryDto>(invToUpdate);
                    return updatedInventoriesDto;
                }

            }
            var inventoryToAdd = _mapper.Map<AddInventoryModel, Inventory>(newInventory);
            var isSuccess = _databaseBoundary.AddInventory(inventoryToAdd);
            if (!isSuccess)
                throw new Exception("Something went wrong, Inventory have not been added!");


            var theNewInventory = _databaseBoundary.GetInventoryById(inventoryToAdd.InventoryId);
            var inventoriesDto = _mapper.Map<Inventory, InventoryDto>(theNewInventory);

            return inventoriesDto;
        }

        public InventoryDto UpdateInventory(UpdateInventoryModel updateInventory, ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;


            var inventoryToUpdate = _databaseBoundary.GetInventoryById((long)updateInventory.InventoryId);
            if (inventoryToUpdate == null)
                throw new NullReferenceException("Inventory not found");


            //delete inventory if empty
            if (updateInventory.Amount <= 0)
            {
                var deletedSuccessfully = DeleteInventory((long)updateInventory.InventoryId, user);
                //TODO: do something with deletedSuccessfully with "error model"
                return null;
            }

            _mapper.Map<UpdateInventoryModel, Inventory>(updateInventory, inventoryToUpdate);


            var isSuccess = _databaseBoundary.UpdateInventory(inventoryToUpdate);
            if (!isSuccess)
                return null;

            var resultInventory = _databaseBoundary.GetInventoryById((long)updateInventory.InventoryId);
            var inventoryDto = _mapper.Map<Inventory, InventoryDto>(resultInventory);

            return inventoryDto;
        }

        public bool DeleteInventory(long inventoryId, ClaimsPrincipal user)
        {
            var inventoryToRemove = _databaseBoundary.GetInventoryById(inventoryId);
            if (inventoryToRemove == null)
                return false;


            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return false;
            var shelf = _databaseBoundary.GetShelfById(inventoryToRemove.ShelfId);
            if (shelf == null)
                throw new NullReferenceException("Shelf not found");
            if (shelf.UserId != userId)
                return false;


            var isSuccess = _databaseBoundary.DeleteInventory(inventoryToRemove);

            return isSuccess;
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// vintage
        public IList<VintageDto> GetAllVintagesByWineId(long wineId)
        {
            var vintages = _databaseBoundary.GetAllVintagesByWineId(wineId);
            IList<VintageDto> vintagesDto = _mapper.Map<Vintage[], IList<VintageDto>>(vintages.ToArray());

            return vintagesDto.OrderBy(v => v.Year).ToList();
        }
        public VintageDto AddVintage(AddVintageModel newVintage)
        {
            int yearNow = Int32.Parse(DateTime.Now.ToString("yyyy"));

            if (newVintage.Year < 1000 || newVintage.Year > yearNow)
                throw new Exception("Invalid year!!");

            //check if the vintage already exist
            var vintages = _databaseBoundary.GetAllVintages(new List<long>() { (long)newVintage.WineId });
            if (vintages.Any(v => v.Year == newVintage.Year))
                return null;

            var vintage = _mapper.Map<AddVintageModel, Vintage>(newVintage);
            var addedVintage = _databaseBoundary.AddVintage(vintage);
            var vintageDto = _mapper.Map<Vintage, VintageDto>(addedVintage);
            return vintageDto;
        }

        public VintageDto UpdateVintage(UpdateVintageModel updateVintage)
        {
            var vintageToUpdate = _databaseBoundary.GetVintageById((long)updateVintage.VintageId);
            if (vintageToUpdate == null)
                return null;


            _mapper.Map<UpdateVintageModel, Vintage>(updateVintage, vintageToUpdate);
            vintageToUpdate.Wine = null;

            var isSuccess = _databaseBoundary.UpdateVintage(vintageToUpdate);
            if (!isSuccess)
                return null;

            var resultVintage = _databaseBoundary.GetVintageById((long)updateVintage.VintageId);
            var vintageDto = _mapper.Map<Vintage, VintageDto>(resultVintage);

            return vintageDto;
        }

        public bool DeleteVintage(long vintageId)
        {
            var vintageToRemove = _databaseBoundary.GetVintageById(vintageId);
            if (vintageToRemove == null)
                return false;

            var isSuccess = _databaseBoundary.DeleteVintage(vintageToRemove);

            return isSuccess;
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// wine list
        public IList<WineListDto> GetUsersWineList(
            ClaimsPrincipal user,
            long shelfId = -1,
            long wineId = -1,
            long districtId = -1,
            long regionId = -1,
            long countryId = -1,
            string startsWith = "",
            long grapeId = -1,
            bool contains = false)
        {
            startsWith ??= "";


            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;
            var usersInventories = _databaseBoundary.GetUsersWineList(
                userId, shelfId, wineId, districtId, regionId, countryId,
                startsWith, grapeId);
            var grades = _databaseBoundary.GetGradesByUserId(userId);


            var wineLists = InventoryListToWineListDto(usersInventories);

            // if there is less than 3 objects, Will get more that contains the name string 
            if (wineLists.Count() <= 2 && !contains)
            {
                contains = true;
                wineLists = GetUsersWineList(user, shelfId, wineId, districtId, regionId, countryId, startsWith,
                    grapeId, contains);
            }

            foreach (var ui in usersInventories)
                ui.Vintage.Grade = grades.FirstOrDefault(i => i.VintageId == ui.VintageId);


            foreach (var wine in wineLists)
            {
                wine.ImageBig = IncomigImageService.GetOriginalImageUrl(wine.WineId);
                wine.ImageThumbnail = IncomigImageService.GetThumbnailImageUrl(wine.WineId);
            }

            //order by year then shelfname
            if (wineLists.Count() > 0)
                foreach (var item in wineLists)
                {
                    if (item.Vintages != null && item.Vintages.Count() > 0)
                    {
                        var temp = item.Vintages;
                        temp = temp.OrderBy(vin => vin.Year).ThenBy(vin => vin.ShelfName, new CustomShelfnameComparer()).ToList();
                        item.Vintages = temp;
                    }
                }
            return wineLists.OrderBy(w => w.WineName).ToList();
        }

        public IList<WineListDto> GetAllWineListWithUserInfo(
            ClaimsPrincipal user,
            long wineId = -1,
            long districtId = -1,
            long regionId = -1,
            long countryId = -1,
            string startsWith = "",
            long grapeId = -1,
            bool containsInstead = false)
        {
            startsWith ??= "";


            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;

            //stage 1 users winelist
            var usersInventories = _databaseBoundary.GetUsersWineList(
                userId, -1, wineId, districtId, regionId, countryId,
                startsWith, grapeId, containsInstead);
            var grades = _databaseBoundary.GetGradesByUserId(userId);


            var wineList = InventoryListToWineListDto(usersInventories);


            // wineList = GetAllVintages(wineList.Take(10).ToList());

            foreach (var ui in usersInventories)
                ui.Vintage.Grade = grades.FirstOrDefault(i => i.VintageId == ui.VintageId);
            //if I got 10 resluts, there is no need to get more 
            if (wineList.Count() >= 10)
            {
                foreach (var wine in wineList)
                {
                    wine.ImageBig = IncomigImageService.GetOriginalImageUrl(wine.WineId);
                    wine.ImageThumbnail = IncomigImageService.GetThumbnailImageUrl(wine.WineId);
                }


                //order by year then shelfname
                foreach (var item in wineList)
                {
                    if (item.Vintages != null && item.Vintages.Count() > 0)
                    {
                        var temp = item.Vintages;
                        temp = temp.OrderBy(vin => vin.Year).ThenBy(vin => vin.ShelfName, new CustomShelfnameComparer()).ToList();
                        item.Vintages = temp;
                    }
                }
                return wineList.Take(10).OrderBy(w => w.WineName).ToList();
            }


            //stage 2 the rest winelist

            var restInventories = _databaseBoundary.GetAllWinesOrBy(
                wineId, districtId, regionId, countryId,
                startsWith, grapeId, containsInstead);

            foreach (var wine in restInventories)
            {
                foreach (var vintage in wine.Vintages)
                {
                    vintage.Grade = grades.FirstOrDefault(i => i.VintageId == vintage.VintageId);
                }
            }

            IList<WineListDto> restWineList = new List<WineListDto>();
            foreach (var wine in restInventories)
            {
                restWineList.Add(new WineListDto()
                {
                    Alcohol = wine.Alcohol,
                    District = _mapper.Map<District, DistrictDto>(wine.District),
                    Region = _mapper.Map<Region, WinelistRegionDto>(wine.District.Region),
                    Country = _mapper.Map<Country, WinelistCountryDto>(wine.District.Region.Country),
                    WineGrapes = _mapper.Map<WineGrape[], IList<WineGrapeDto>>(wine.WineGrapes.ToArray()),
                    Producer = wine.Producer,
                    WineId = wine.WineId,
                    WineName = wine.Name,
                    Vintages = null,
                    //Vintages = wine.Vintages.Select(x => new WineListVintagesDtio
                    //{
                    //    WineId = x.WineId,
                    //    VintageId = x.VintageId,
                    //    Year = x.Year,
                    //    EAN = x.EAN,

                    //    InventoryId = 0,
                    //    Amount = 0,
                    //    ShelfId = 0,
                    //    ShelfName = null,


                    //    Grade = x.Grade != null
                    //        ? new GradeDto()
                    //        {
                    //            GradeTableId = (long) x.Grade.GradeTableId,
                    //            Grade = x.Grade.Grade,
                    //            Comment = x.Grade.Comment
                    //        }
                    //        : null,
                    //}).ToList(),
                });
            }


            foreach (var item in restWineList)
            {
                if (wineList.Any(w => w.WineId == item.WineId)) continue;

                wineList.Add(item);

                if (wineList.Count() >= 10) break;
            }


            // if there is less than 3 objects, Will get more that contains the name string 
            if (wineList.Count() <= 2 && !containsInstead)
            {
                containsInstead = true;
                wineList = GetAllWineListWithUserInfo(user, wineId, districtId, regionId, countryId, startsWith,
                    grapeId, containsInstead);
            }
            wineList = wineList.Take(10).ToList();

            foreach (var wine in wineList)
            {
                wine.ImageBig = IncomigImageService.GetOriginalImageUrl(wine.WineId);
                wine.ImageThumbnail = IncomigImageService.GetThumbnailImageUrl(wine.WineId);
            }

            //order by year then shelfname
            if (wineList.Count() > 0)
                foreach (var item in wineList)
                {
                    if (item.Vintages != null && item.Vintages.Count() > 0)
                    {
                        var temp = item.Vintages;
                        temp = temp.OrderBy(vin => vin.Year).ThenBy(vin => vin.ShelfName, new CustomShelfnameComparer()).ToList();
                        item.Vintages = temp;
                    }
                }

            return wineList.OrderBy(w => w.WineName).ToList();
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// images

        public byte[] GetBigImage(long wineId)
        {
            return _imageService.GetBigImage(wineId);
        }

        public byte[] GetThumbnailImage(long wineId)
        {
            return _imageService.GetThumbnailImage(wineId);
        }
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// mock
        public void Mock()
        {
            _databaseBoundary.Mock();
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// Grades
        public GradeDto AddGrade(AddGradeModel AddGrade, ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;

            var checkIfExist = _databaseBoundary.GetGradeByUserIdAndVintageId(userId, (long)AddGrade.VintageId);
            if (checkIfExist != null)
                throw new ArgumentException("This grade is already exist, try to update it!");

            var gradeToAdd = _mapper.Map<AddGradeModel, GradeTable>(AddGrade);

            gradeToAdd.UserId = userId;

            var isSusscced = _databaseBoundary.AddGrade(gradeToAdd);
            if (!isSusscced)
                throw new Exception("something went wrong ");


            var gradeDto = _mapper.Map<GradeTable, GradeDto>(gradeToAdd);


            return gradeDto;


            //var gradeToUpdate = _databaseBoundary.GetGradeById(updateGrade.GradeId);
            //if (gradeToUpdate != null)
            //    throw new NullReferenceException("Grades already exist!");

            //if (gradeToUpdate.UserId != userId)
            //    throw new AccessViolationException("Not your Grades");


            //var isSucced = _databaseBoundary.UpdateGrade(gradeToUpdate);
            //if (!isSucced)
            //    throw new Exception("Something went wrong");
        }

        public GradeDto UpdateGrade(UpdateGradeModel UpdateGrade, ClaimsPrincipal user)
        {
            var userId = _userService.GetUserIdFromSession(user);
            if (userId == null)
                return null;

            var GradeToUpdate = _databaseBoundary.GetGradeById((long)UpdateGrade.GradeId);

            if (GradeToUpdate == null)
                throw new ArgumentException("This grade do not exist!");
            if (GradeToUpdate.UserId != userId)
                throw new AccessViolationException("Not yours grade to update!");


            _mapper.Map<UpdateGradeModel, GradeTable>(UpdateGrade, GradeToUpdate);


            var isSusscced = _databaseBoundary.UpdateGrade(GradeToUpdate);
            if (!isSusscced)
                throw new Exception("something went wrong ");


            var gradeDto = _mapper.Map<GradeTable, GradeDto>(GradeToUpdate);


            return gradeDto;
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// helper methds

        private string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("string is Empty");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        private IList<WineListDto> InventoryListToWineListDto(IList<Inventory> inventories)
        {
            var inventoriesGroupedByWineId = inventories.GroupBy(ii => new { ii.Vintage.WineId });
            IList<WineListDto> wineList = new List<WineListDto>();
            foreach (var groupingByClassA in inventoriesGroupedByWineId)
            {
                var doItOnce = true;
                IList<WineListVintagesDtio> vintages = new List<WineListVintagesDtio>();
                wineList.Add(new WineListDto() { WineId = groupingByClassA.Key.WineId, });
                //iterating through values
                foreach (var classA in groupingByClassA)
                {
                    var toAdd = new WineListVintagesDtio()
                    {
                        WineId = classA.Vintage.WineId,
                        VintageId = classA.VintageId,
                        Year = classA.Vintage.Year,
                        EAN = classA.Vintage.EAN,
                        InventoryId = classA.InventoryId,
                        Amount = classA.Amount,
                        ShelfId = classA.ShelfId,
                        ShelfName = classA.Shelf.ShelfName,

                        Grade = classA.Vintage.Grade != null
                            ? new GradeDto()
                            {
                                GradeTableId = classA.Vintage.Grade != null
                                    ? (long)classA.Vintage.Grade.GradeTableId
                                    : 0,
                                Grade = classA.Vintage.Grade != null ? classA.Vintage.Grade.Grade : 0,
                                Comment = classA.Vintage.Grade?.Comment,
                            }
                            : null,
                    };
                    vintages.Add(toAdd);
                    if (wineList.Count == 0 || !doItOnce) continue;
                    wineList.Last().Producer = classA.Vintage.Wine.Producer;
                    wineList.Last().Alcohol = classA.Vintage.Wine.Alcohol;
                    wineList.Last().WineName = classA.Vintage.Wine.Name;
                    wineList.Last().District = _mapper.Map<District, DistrictDto>(classA.Vintage.Wine.District);
                    wineList.Last().Region =
                        _mapper.Map<Region, WinelistRegionDto>(classA.Vintage.Wine.District.Region);
                    wineList.Last().Country =
                        _mapper.Map<Country, WinelistCountryDto>(classA.Vintage.Wine.District.Region.Country);
                    wineList.Last().WineGrapes =
                        _mapper.Map<WineGrape[], IList<WineGrapeDto>>(classA.Vintage.Wine.WineGrapes.ToArray());

                    doItOnce = false;
                }

                if (wineList.Count != 0)
                    wineList.Last().Vintages = vintages;
            }

            return wineList;
        }

        private IList<WineListDto> GetAllVintages(IList<WineListDto> wineList)
        {
            var wineIds = new List<long>();
            foreach (var wine in wineList)
                wineIds.Add(wine.WineId);

            var vintages = _databaseBoundary.GetAllVintages(wineIds);

            foreach (var wine in wineList)
                foreach (var vintage in vintages)
                    if (wine.WineId == vintage.WineId && !wine.Vintages.Any(v => v.VintageId == vintage.VintageId))
                    {
                        wine.Vintages.Add(new WineListVintagesDtio
                        {
                            WineId = wine.WineId,
                            VintageId = vintage.VintageId,
                            Year = vintage.Year,
                            EAN = vintage.EAN,
                        });
                    }

            return wineList;
        }


    }
}