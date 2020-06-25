using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using basement.core.Entities.GeneralSection;
using basement.core.Entities.UserSection;
using basement.infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;


namespace basement.database
{
    public class DatabaseBoundary : IDatabaseBoundary
    {
        private readonly DatabaseContext _databaseContext;


        public DatabaseBoundary(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// metadata
        public IList<Country> GetAllCountriesRegionsDistricts()
        {
            return _databaseContext.Countries
                .Include(c => c.Regions)
                .ThenInclude(r => r.Districts)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Grape> GetAllGrapes()
        {
            var grapes = _databaseContext.Grapes
                .AsNoTracking()
                .ToList();

            return grapes;
        }
        public Country AddCountry(Country country)
        {
            _databaseContext.Countries.Add(country);
            var isSucced = SaveChanges();
            return isSucced ? country : null;

        }

        public Region AddRegion(Region region)
        {
            _databaseContext.Regions.Add(region);
            var isSucced = SaveChanges();
            return isSucced ? region : null;
        }

        public District AddDistrict(District district)
        {
            _databaseContext.Districts.Add(district);
            var isSucced = SaveChanges();
            return isSucced ? district : null;
        }
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// Wine  
        public IList<Wine> GetAllWinesOrBy(long wineId, long districtId, long regionId,
            long countryId, string startsWith, long grapeId, bool containsInstead)
        {
            var wines = _databaseContext.Wines
                .Where(i => containsInstead
                    ? i.Name.ToLower().Contains(startsWith.ToLower())
                    : i.Name.ToLower().StartsWith(startsWith.ToLower()))
                .Where(wi => wineId == -1 || wi.WineId == wineId)
                .Where(wi => districtId == -1 || wi.District.DistrictId == districtId)
                .Where(wi => regionId == -1 || wi.District.RegionId == regionId)
                .Where(wi => countryId == -1 || wi.District.Region.CountryId == countryId)
                .Where(wi => grapeId == -1 || wi.WineGrapes.Any(g => g.GrapeId == grapeId))
                .Include(w => w.District)
                .ThenInclude(d => d.Region)
                .ThenInclude(r => r.Country)
                .Include(w => w.WineGrapes)
                .ThenInclude(wg => wg.Grape)
                .Include(w => w.Vintages)
                .OrderBy(x => x.Name == startsWith)
                .Take(10)
                .ToList();

            return wines;
        }

        public Wine GetWineById(long wineId)
        {
            return _databaseContext.Wines
                .Include(w => w.District)
                .Include(w => w.WineGrapes)
                .ThenInclude(wg => wg.Grape)
                .AsNoTracking()
                .FirstOrDefault(x => x.WineId == wineId);
        }


        public bool CheckIfWineExistByName(string wineName)
        {
            return _databaseContext.Wines.Any(w => w.Name == wineName);
        }


        public Wine AddWine(Wine newWine)
        {
            _databaseContext.Wines.Add(newWine);
            var isSucced = SaveChanges();
            return isSucced
                ? _databaseContext.Wines
                    .Include(w => w.District)
                    .Include(w => w.WineGrapes)
                    .ThenInclude(wg => wg.Grape)
                    .FirstOrDefault(w => w.WineId == newWine.WineId)
                : null;
        }

        public bool UpdateWine(Wine updatedWine)
        {
            _databaseContext.WineGrapes.RemoveRange(_databaseContext.WineGrapes
                .Where(w => w.WineId == updatedWine.WineId).ToList());

            _databaseContext.Wines.Update(updatedWine);
            return SaveChanges();
        }

        public bool DeleteWine(Wine wineToRemove)
        {
            _databaseContext.Wines.Remove(wineToRemove);
            return SaveChanges();
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// Vintage
        public IList<Vintage> GetAllVintagesByWineId(long wineId)
        {
            var vintages = _databaseContext.Vintages
                .Where(v => v.WineId == wineId)
                .Include(v => v.Wine)
                .ThenInclude(w => w.District)
                .Include(v => v.Wine)
                .ThenInclude(w => w.WineGrapes)
                .ThenInclude(g => g.Grape)
                .ToList();
            return vintages;
        }

        public Vintage GetVintageById(long vintageId)
        {
            return _databaseContext.Vintages
                .AsNoTracking()
                .FirstOrDefault(v => v.VintageId == vintageId);
        }

        public IList<Vintage> GetAllVintages(IList<long> wineIds)
        {
            return _databaseContext.Vintages.Where(v => wineIds.Contains(v.WineId)).ToList();

            ////var itemEntity = db.ItemsEntity.Where(m => listOfIds.Contains(m.item_id));

            //IList<Vintage> vintages = new List<Vintage>();

            //foreach (var wineid in wineIds)
            //{
            //    vintages.Add(_databaseContext.Vintages
            //    .AsNoTracking()
            //    .FirstOrDefault(v => v.WineId == wineid));
            //}
            //return vintages;
        }

        public Vintage AddVintage(Vintage newVintages)
        {
            _databaseContext.Vintages.Add(newVintages);
            var isSucced = SaveChanges();
            return isSucced ? newVintages : null;
        }

        public bool UpdateVintage(Vintage updatedVintage)
        {
            _databaseContext.Vintages.Update(updatedVintage);
            return SaveChanges();
        }

        public bool DeleteVintage(Vintage vintageToDelete)
        {
            _databaseContext.Vintages.Remove(vintageToDelete);
            return SaveChanges();
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// User

        //public User GetUserByUserName(string userName)
        //{
        //    return _databaseContext.Users.FirstOrDefault(u => u.Username == userName);
        //}
        public User GetUserById(string id)
        {
            return _databaseContext.Users.FirstOrDefault(u => u.UserId == id);
        }

        public bool AddUser(User user)
        {
            _databaseContext.Users.Add(user);
            var isSucced = SaveChanges();
            return isSucced;
        }

        public bool UpdateUser(User user)
        {
            _databaseContext.Users.Update(user);
            return SaveChanges();
        }

        public bool DeleteUser(User user)
        {
            _databaseContext.Users.Remove(user);
            return SaveChanges();
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// Shelf
        public IList<Shelf> GetAllShelves(string userId)
        {
            var shelves = _databaseContext.Shelves
                .Where(s => s.UserId == userId)
                .AsNoTracking()
                .ToList();

            return shelves;
        }


        public Shelf GetShelfById(long shelfId)
        {
            return _databaseContext.Shelves
                .Where(s => s.ShelfId == shelfId)
                .AsNoTracking()
                .FirstOrDefault(s => s.ShelfId == shelfId);
        }

        public Shelf AddShelf(Shelf newShelve)
        {
            _databaseContext.Shelves.Add(newShelve);
            var isSucced = SaveChanges();
            return isSucced ? newShelve : null;
        }


        public bool UpdateShelf(Shelf updateShelf)
        {
            _databaseContext.Shelves.Update(updateShelf);
            return SaveChanges();
        }

        public bool DeleteShelf(Shelf shelf)
        {
            _databaseContext.Shelves.Remove(shelf);
            return SaveChanges();
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// Inventory
        /// 
        public IList<Inventory> GetUsersInventory(string userId)
        {
            return _databaseContext.Inventories
                .Include(i => i.Vintage)
                .ThenInclude(v => v.Wine)
                .ThenInclude(w => w.District)
                .Include(i => i.Vintage)
                .ThenInclude(v => v.Wine)
                .ThenInclude(w => w.WineGrapes)
                .ThenInclude(wg => wg.Grape)
                .Where(i => i.Shelf.UserId == userId)
                .ToList();
        }



        public Inventory GetInventoryById(long inventoryId)
        {
            return _databaseContext.Inventories
                .Include(i => i.Vintage)
                .ThenInclude(v => v.Wine)
                .ThenInclude(w => w.District)
                .Include(i => i.Vintage)
                .ThenInclude(v => v.Wine)
                .ThenInclude(w => w.WineGrapes)
                .ThenInclude(wg => wg.Grape)
                .FirstOrDefault(i => i.InventoryId == inventoryId);
        }

        public bool AddInventory(Inventory newInventories)
        {
            _databaseContext.Inventories.Add(newInventories);
            var isSucced = SaveChanges();
            return isSucced;
        }


        public bool UpdateInventory(Inventory updateInventory)
        {
            _databaseContext.Inventories.Update(updateInventory);
            return SaveChanges();
        }

        public bool DeleteInventory(Inventory inventory)
        {
            _databaseContext.Inventories.Remove(inventory);
            return SaveChanges();
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// helper
        private bool SaveChanges()
        {
            try
            {
                _databaseContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// winelist
        public IList<Inventory> GetUsersWineList(
            string userId = "-1",
            long shelfId = -1,
            long wineId = -1,
            long districtId = -1,
            long regionId = -1,
            long countryId = -1,
            string startsWith = "",
            long grapeId = -1,
            bool containsInstead = false)
        {
            IList<Inventory> wineList = _databaseContext.Inventories
                .Where(i => i.Shelf.UserId == userId || userId == "-1")
                .Where(i => i.ShelfId == shelfId || shelfId == -1)
                .Where(i => grapeId == -1 || i.Vintage.Wine.WineGrapes.Any(g => g.GrapeId == grapeId))
                .Where(i => i.Vintage.Wine.WineId == wineId || wineId == -1)
                .Where(i => i.Vintage.Wine.DistrictId == districtId || districtId == -1)
                .Where(i => i.Vintage.Wine.District.RegionId == regionId || regionId == -1)
                .Where(i => i.Vintage.Wine.District.Region.CountryId == countryId || countryId == -1)
                .Where(i => containsInstead
                    ? i.Vintage.Wine.Name.ToLower().Contains(startsWith.ToLower())
                    : i.Vintage.Wine.Name.ToLower().StartsWith(startsWith.ToLower()))

                //.IncludeFilter(i => i.Vintage.Grades.FirstOrDefault(x=>x.UserId == userId))
                .Include(i => i.Shelf)
                .Include(i => i.Vintage)
                .ThenInclude(v => v.Wine)
                .ThenInclude(w => w.District)
                .ThenInclude(d => d.Region)
                .ThenInclude(r => r.Country)
                .Include(i => i.Vintage)
                .ThenInclude(v => v.Wine)
                .ThenInclude(w => w.WineGrapes)
                .ThenInclude(wg => wg.Grape)
                .OrderBy(x => x.Vintage.Wine.Name == startsWith)
                .ToList();


            return wineList;
        }

        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// grades
        public GradeTable GetGradeById(long gradeId)
        {
            var grade = _databaseContext.Grades
                .AsNoTracking()
                .FirstOrDefault(g => g.GradeTableId == gradeId);

            return grade;
        }

        public bool UpdateGrade(GradeTable grade)
        {
            _databaseContext.Grades.Update(grade);
            return SaveChanges();
        }

        public bool AddGrade(GradeTable grade)
        {
            _databaseContext.Grades.Add(grade);
            return SaveChanges();
        }

        public IList<GradeTable> GetGradesByUserId(string userId)
        {
            return _databaseContext.Grades
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .ToList();
        }

        public GradeTable GetGradeByUserIdAndVintageId(string userId, long vintageId)
        {
            return _databaseContext.Grades
                .AsNoTracking()
                .FirstOrDefault(g => g.UserId == userId && g.VintageId == vintageId);
        }


        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        ////////////////////////////////
        /// Mock
        public void Mock()
        {
            //if (grapes.Any(g => string.Compare(
            //    g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8))).Replace(' ', '-'),
            //    grape.GrapeName.Substring(0, ((int)(grape.GrapeName.Length * 0.8))).Replace(' ', '-'),
            //    CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)) continue;
            //some info
            ////list all wines which exist in both lists ---->   //===> there is not so I deleted the code <=== 


            Random rnd = new Random();

            //get the two dataset 
            var systembolagetDataset = LoadSystemBolagetDataset();
            var kaggledataset = LoadKaggleDataset();


            //delete and recreate the database 
            _databaseContext.Database.EnsureDeleted();
            _databaseContext.Database.EnsureCreated();

            //delete all images
            {
                char separator = Path.DirectorySeparatorChar;
                string imageFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages";
                string thumbnailPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages" + separator + "thumbnail";
                string bigPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages" + separator + "big";
                string originalPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + separator + "wineImages" + separator + "original";
                System.IO.DirectoryInfo di = new DirectoryInfo(thumbnailPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.Name == "-1.png") continue;
                    file.Delete();
                }
                di = new DirectoryInfo(bigPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.Name == "-1.jpg") continue;
                    file.Delete();
                }
                di = new DirectoryInfo(originalPath);
                foreach (FileInfo file in di.GetFiles())
                {
                    if (file.Name == "-1.jpg") continue;
                    file.Delete();
                }
            }




            //add countries, regions and districts
            AddCountriesRegionsDistricts(
                systembolagetDataset,
                kaggledataset,
                out var countries,
                out var regions,
                out var districts);

            //add grapes
            AddGrapes(systembolagetDataset, kaggledataset, out var grapes);


            //add wines
            List<Wine> wines = addWines(
                systembolagetDataset,
                kaggledataset,
                grapes, countries, regions, districts);

            List<Vintage> vintages = addVintages(
                systembolagetDataset, kaggledataset, wines);


            ///////////////////////////////////////////////////
            ///////////////////////////////////////////////////
            ///////////////////////////////////////////////////
            ///////////////////////////////////////////////////


            //  List<string> userIdz = new List<string>();
            List<User> users = new List<User>()
            {
                new User
                {
                    UserId = "Av4ivM6LMlRVz7W5YbgilBXUK5d2",
                    Username = "user 1"
                },
                new User
                {
                    UserId = "TCs7GexCzcZbKFesiMcgBramkY42",
                    Username = "user 2"
                },
                new User
                {
                    UserId = "yqSCxXO90jbNncV7qT6fKPOLfbz1",
                    Username = "user 3"
                },
                new User
                {
                    UserId = "TohQThgjkNTbgQzY2wvEQUVhWAp2",
                    Username = "user 4"
                },
                new User
                {
                    UserId = "OmzQPl8ejiPhu1YDRvd3P13esYV2",
                    Username = "user 5"
                }
            };
            _databaseContext.Users.AddRange(users);
            _databaseContext.SaveChanges();


            ///////////////////////////////////////////////////
            ///////////add shelves////////////////////////////
            ///////////////////////////////////////////////////
            ///////////////////////////////////////////////////

            var shelves = new List<Shelf>();
            for (int i = 0; i < 3; i++)
            {
                shelves.Add(new Shelf
                {
                    ShelfName = "shelf " + i + " 1st user",
                    UserId = users[0].UserId
                });
            }

            for (int i = 0; i < 25; i++)
            {
                var user = users[rnd.Next(1, users.Count - 1)];

                shelves.Add(new Shelf
                {
                    ShelfName = "shelf " + i + ":" + user.Username,
                    UserId = user.UserId
                });
            }

            _databaseContext.AddRange(shelves);
            _databaseContext.SaveChanges();


            var firstUserVIntagesIdz = new List<long>();
            var inventories = new List<Inventory>();

            for (int i = 0; i < 150; i++)
            {
                inventories.Add(new Inventory
                {
                    Amount = rnd.Next(1, 50),
                    ShelfId = shelves[rnd.Next(shelves.Count - 1)].ShelfId,

                    VintageId = vintages[rnd.Next(vintages.Count - 1)].VintageId
                });


                var firstUserVintageId = vintages[rnd.Next(vintages.Count - 1)].VintageId;
                inventories.Add(new Inventory()
                {
                    Amount = rnd.Next(60, 110),
                    ShelfId = shelves[rnd.Next(0, 2)].ShelfId,

                    VintageId = firstUserVintageId
                });
                firstUserVIntagesIdz.Add(firstUserVintageId);
            }

            _databaseContext.Inventories.AddRange(inventories);
            _databaseContext.SaveChanges();


            ///////////////////////////////////////////////////
            ///////////add grade////////////////////////////
            ///////////////////////////////////////////////////
            ///////////////////////////////////////////////////

            var Gradezz = new List<GradeTable>()
            {
                new GradeTable()
                {
                    Grade = 2,
                    Comment = "vintageid " + vintages[0].VintageId + " and first user ",
                    UserId = users[0].UserId,
                    VintageId = firstUserVIntagesIdz[0]
                },
                new GradeTable()
                {
                    Grade = 2,
                    Comment = "vintageid " + vintages[1].VintageId + " and first user ",
                    UserId = users[0].UserId,
                    VintageId = firstUserVIntagesIdz[1]
                }
            };
            for (int i = 0; i < 200; i++)
            {
                var vintage = vintages[rnd.Next(vintages.Count() - 1)];
                var user = users[rnd.Next(users.Count() - 1)];
                Gradezz.Add(new GradeTable
                {
                    Grade = rnd.Next(1, 5),
                    UserId = user.UserId,
                    VintageId = vintage.VintageId,
                    Comment = "vintageid " + vintage.VintageId + " for " + user.Username,
                });
            }

            _databaseContext.Grades.AddRange(Gradezz);
            _databaseContext.SaveChanges();

            Console.WriteLine(firstUserVIntagesIdz[0]);
            Console.WriteLine(firstUserVIntagesIdz[1]);
        }


        #region Add Country, Region and district

        private void AddCountriesRegionsDistricts(List<Artikel> systembolagetDataset, List<grapsfromjson> kaggledatase,
            out List<Country> countriesOut, out List<Region> regionsOut, out List<District> districtsOut)
        {
            AddSystemBolagetCountryRegionDistrict(systembolagetDataset, out var countries, out var regions,
                out var districts);
            //AddKaggleCountryRegionDistrict(kaggledatase, countries, regions, districts);

            countriesOut = countries;
            regionsOut = regions;
            districtsOut = districts;
        }

        private void AddSystemBolagetCountryRegionDistrict(List<Artikel> systembolagetDataset,
            out List<Country> countriesOut, out List<Region> regionsOut, out List<District> districtsOut)
        {
            ////////add countries
            var countries = new List<Country>();
            foreach (var wine in systembolagetDataset)
            {
                if (!countries.Any(g => string.Equals(g.CountryName, wine.Ursprunglandnamn, StringComparison.OrdinalIgnoreCase)) &&
                    !string.IsNullOrEmpty(wine.Ursprunglandnamn))
                {
                    countries.Add(new Country
                    {
                        CountryName = FirstLetterToUpperCase(wine.Ursprunglandnamn.ToLower())
                    });
                }
            }

            _databaseContext.Countries.AddRange(countries);
            _databaseContext.SaveChanges();


            ////////add regions
            var regions = new List<Region>();
            foreach (var item in systembolagetDataset)
            {
                var regionName = "Okänt region";

                var theC = countries.FirstOrDefault(x => string.Equals(x.CountryName, item.Ursprunglandnamn, StringComparison.OrdinalIgnoreCase));

                if (!string.IsNullOrEmpty(item.Ursprung))
                    regionName = item.Ursprung;


                if (!regions.Any(r => string.Equals(r.RegionName, regionName, StringComparison.OrdinalIgnoreCase) && r.CountryId == theC.CountryId))
                {
                    regions.Add(new Region
                    {
                        RegionName = FirstLetterToUpperCase(regionName.ToLower()),
                        CountryId = theC.CountryId
                    });
                }
            }

            _databaseContext.Regions.AddRange(regions);
            _databaseContext.SaveChanges();

            ////////add districts
            var districts = new List<District>();
            foreach (var region in regions)
            {
                districts.Add(new District
                {
                    DistrictName = "Okänt distrikt",
                    RegionId = region.RegionId
                });
            }

            _databaseContext.Districts.AddRange(districts);
            _databaseContext.SaveChanges();

            regionsOut = regions;
            countriesOut = countries;
            districtsOut = districts;
        }

        private void AddKaggleCountryRegionDistrict(List<grapsfromjson> kaggledataset, List<Country> countries,
            List<Region> regions, List<District> districts)
        {
            var lookuptable = LoadCountryLookUpTable();
            ////////add countries
            var KaggleCountries = new List<Country>();
            foreach (var wine in kaggledataset)
            {
                string countryNameToAdd = "Okänt ursprung";

                if (!string.IsNullOrEmpty(wine.country))
                    countryNameToAdd = wine.country;

                string countrySwe = lookuptable.FirstOrDefault(x => string.Equals(x.English, countryNameToAdd, StringComparison.OrdinalIgnoreCase)).Swedish;
                if (countrySwe == null)
                    throw new Exception("in add kaggle countries, must fix the countries lookup table");

                if (countries.Any(c => string.Equals(c.CountryName, countrySwe, StringComparison.OrdinalIgnoreCase))
                    || KaggleCountries.Any(c =>
                        string.Equals(c.CountryName, countrySwe, StringComparison.OrdinalIgnoreCase))) continue;

                KaggleCountries.Add(new Country
                {
                    CountryName = FirstLetterToUpperCase(countrySwe.ToLower())
                });
            }

            _databaseContext.Countries.AddRange(KaggleCountries);
            _databaseContext.SaveChanges();

            countries.AddRange(KaggleCountries);

            ////////add regions 
            var kaggleRegions = new List<Region>();


            foreach (var item in kaggledataset)
            {
                string countryName = "Okänt ursprung";
                var regionNameToAdd = "Okänt region";


                if (!string.IsNullOrEmpty(item.country))
                    countryName = item.country;


                var countryId = countries.First(co => string.Equals(co.CountryName,
                    lookuptable.First(c => string.Equals(c.English, countryName, StringComparison.OrdinalIgnoreCase)).Swedish, StringComparison.OrdinalIgnoreCase)).CountryId;

                if (!string.IsNullOrEmpty(item.region_2))
                    regionNameToAdd = item.region_2;
                else if (!string.IsNullOrEmpty(item.province))
                    regionNameToAdd = item.province;


                if (regions.Any(r => string.Equals(r.RegionName, regionNameToAdd, StringComparison.OrdinalIgnoreCase) && r.CountryId == countryId)
                    || kaggleRegions.Any(r => string.Equals(r.RegionName, regionNameToAdd, StringComparison.OrdinalIgnoreCase) && r.CountryId == countryId)) continue;


                var regionToAdd = new Region
                {
                    RegionName = FirstLetterToUpperCase(regionNameToAdd.ToLower()),
                    CountryId = countryId
                };

                kaggleRegions.Add(regionToAdd);
            }

            _databaseContext.Regions.AddRange(kaggleRegions);
            _databaseContext.SaveChanges();

            regions.AddRange(kaggleRegions);

            ////////add districts 
            var kaggleDistricts = new List<District>();
            foreach (var item in kaggledataset)
            {
                string countryName = "Okänt ursprung";
                var regionName = "Okänt region";
                var districtNameToAdd = "Okänt distrikt";


                if (!string.IsNullOrEmpty(item.country))
                    countryName = item.country;


                if (!string.IsNullOrEmpty(item.region_2))
                    regionName = item.region_2;
                else if (!string.IsNullOrEmpty(item.province))
                    regionName = item.province;


                var countryId = countries.First(co => string.Equals(co.CountryName,
                    lookuptable.First(c => string.Equals(c.English, countryName, StringComparison.OrdinalIgnoreCase)).Swedish, StringComparison.OrdinalIgnoreCase)).CountryId;

                var regionId = regions.First(r
                    => string.Equals(r.RegionName, regionName, StringComparison.OrdinalIgnoreCase)
                       && r.CountryId == countryId).RegionId;

                if (!string.IsNullOrEmpty(item.region_1))
                    districtNameToAdd = item.region_1;

                // var regionIdToFindDistrict = regions.FirstOrDefault(r => string.Equals(r.RegionName, regionName)).RegionId;
                //var defaultDistrict = districts.FirstOrDefault(r => r.RegionId == regionId && r.DistrictName == "Okänt distrikt");
                //if (defaultDistrict != null) continue;
                //districtNameToAdd = "Okänt distrikt";


                if (districts.Any(d =>
                        string.Equals(d.DistrictName, districtNameToAdd, StringComparison.OrdinalIgnoreCase) &&
                        d.RegionId == regionId)
                    || kaggleDistricts.Any(d =>
                        string.Equals(d.DistrictName, districtNameToAdd, StringComparison.OrdinalIgnoreCase) &&
                        d.RegionId == regionId)) continue;

                kaggleDistricts.Add(new District
                {
                    DistrictName = FirstLetterToUpperCase(districtNameToAdd.ToLower()),
                    RegionId = regionId
                });
            }

            _databaseContext.Districts.AddRange(kaggleDistricts);
            _databaseContext.SaveChanges();

            districts.AddRange(kaggleDistricts);
        }

        #endregion


        #region AddGrapes

        private void AddGrapes(List<Artikel> systembolagetDataset, List<grapsfromjson> kaggledataset,
            out List<Grape> grapesOut)
        {
            //var KaggleGrapes = AddKaggleGrapes(kaggledataset);


            var SystemBolagetGrapes = AddSystemBolagetGrapes(systembolagetDataset);

            var grapes = new List<Grape>(SystemBolagetGrapes);
            //foreach (var grape in KaggleGrapes)
            //{

            //    if (doCompare(grapes, grape.GrapeName)) continue;
            //    grapes.Add(grape);
            //}

            _databaseContext.Grapes.AddRange(grapes);
            _databaseContext.SaveChanges();

            grapesOut = grapes;
        }

        private bool doCompare(List<Grape> grapes, string GrapeName)
        {
            //if (GrapeName == "Arago�es"|| GrapeName == "aragones")
            //    Console.WriteLine();
            //if ((grapes.Any(gg
            //    => doNormalize(gg.GrapeName).Substring(0, ((int)(gg.GrapeName.Length * 0.7))).Replace(' ', '-').ToLower()
            //    .StartsWith(doNormalize(GrapeName).Substring(0,
            //    ((int)(GrapeName.Length * 0.7))).Replace(' ', '-').ToLower()
            //    , true, CultureInfo.CurrentCulture))))
            if (grapes.Any(g => string.Compare(
                g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8))).Replace(' ', '-'),
                GrapeName.Substring(0, ((int)(GrapeName.Length * 0.8))).Replace(' ', '-'),
                CultureInfo.CurrentCulture,
                CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0))
            {
                return true;
            }


            return false;
        }

        private string doNormalize(string s)
        {
            s = s.Replace('ü', 'u');
            s = s.Replace('ê', 'e');
            s = s.Replace('è', 'e');
            s = s.Replace('ö', 'o');
            s = s.Replace('ò', 'o');
            s = s.Replace('ñ', 'n');
            s = s.Replace('ä', 'a');
            s = s.Replace('å', 'a');
            s = s.Replace('í', 'i');
            s = s.Replace('β', 'b');


            return s;
        }

        private List<Grape> AddKaggleGrapes(List<grapsfromjson> kaggledataset)
        {
            char[] spearator = { '-', ',' };
            List<Grape> grapes = new List<Grape>();
            // List<grapeswithcount> grapeswithcount = new List<grapeswithcount>();

            foreach (var oldLine in kaggledataset)
            {
                if (string.IsNullOrEmpty(oldLine.variety)) continue;

                var line = oldLine.variety;
                line = line.Replace("Xarel-lo", "xarel.lo", StringComparison.OrdinalIgnoreCase);
                line = line.Replace("G-S-M", "G.S.M", StringComparison.OrdinalIgnoreCase);

                var grapesInLine = line.Split(spearator);

                foreach (var oldGrape in grapesInLine)
                {
                    var grape = oldGrape.Trim();


                    if (string.IsNullOrWhiteSpace(grape)) continue;
                    //|| grapes.Any(g
                    //=> string.Compare(
                    //    g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8))).Replace(' ', '-'),
                    //    grape.Substring(0, ((int)(grape.Length * 0.8))).Replace(' ', '-'),
                    //    CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)) continue;

                    if (doCompare(grapes, grape)) continue;
                    grapes.Add(new Grape
                    {
                        GrapeName = FirstLetterToUpperCase(grape.ToLower())
                    });
                }
            }

            return grapes;
        }

        private List<Grape> AddSystemBolagetGrapes(List<Artikel> systembolagetDataset /*, List<Grape> grapesKaggle*/)
        {
            string[] spearator =
            {
                ",", " även ", " och en mindre del", " och ", "samt en liten del", "en liten del", " samt ",
                "Huvudsakligen", "Egendomen är planterad med", "Till största delen", "Vingården är planterad med"
            };
            var grapes = new List<Grape>();
            //var regularExpression = new Regex(@"");
            //list all "RavarorBeskrivning" 
            var RavarorBeskrivning = new List<string>();
            var sbGrapes = new List<string>();
            foreach (var item in systembolagetDataset)
            {
                if (
                    item.Varugrupp != "Rött vin" &&
                    item.Varugrupp != "Vitt vin" &&
                    item.Varugrupp != "Mousserande vin" &&
                    item.Varugrupp != "Rosévin" &&
                    // wineJson.Varugrupp != "Blandl�dor vin" &&
                    item.Varugrupp != "Röda - lägre alkoholhalt" &&
                    item.Varugrupp != "Rosé - lägre alkoholhalt" &&
                    item.Varugrupp != "Vita - lägre alkoholhalt"
                ) continue;
                if (item.RavarorBeskrivning == null) continue;
                if (RavarorBeskrivning.FirstOrDefault(r =>
                        string.Equals(r, item.RavarorBeskrivning, StringComparison.OrdinalIgnoreCase)) !=
                    null) continue;

                RavarorBeskrivning.Add(item.RavarorBeskrivning);
            }


            foreach (var Oldline in RavarorBeskrivning)
            {
                var line = Oldline;
                if (line[line.Count() - 1] == '%')
                    line = fixTheLine(line);


                if (string.IsNullOrEmpty(line)) continue;


                if (line[line.Count() - 1] == '.')
                    line = line.Remove(line.Count() - 1, 1);

                if (line[0] == '.')
                    line = line.Remove(0, 1);

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == ',')
                    {
                        if (!char.IsDigit(line[i - 1])) continue;
                        if (!char.IsDigit(line[i + 1])) continue;
                        line = line.Remove(i, 1);
                        line = line.Insert(i, ".");
                    }

                    if (line[i] == '%')
                    {
                        if (char.IsLetterOrDigit(line[i + 1]))
                            line = line.Insert(i + 1, " ");
                        if (char.IsWhiteSpace(line[i - 1]))
                            line = line.Remove(i - 1, 1);
                    }
                }

                line = line.Replace(" and ", " och ");
                line = line.Replace("chardonnay.", "chardonnay,");
                line = line.Replace("Xarel-lo", "xarel.lo", StringComparison.OrdinalIgnoreCase);
                line = line.Replace("corvina 20% rondinella", "corvina, 20% rondinella");
                line = line.Replace("garnacha tinta (grenache) 25% syrah", "garnacha tinta (grenache), 25% syrah");

                var oneLineGrapes = line.Split(spearator, StringSplitOptions.None);


                var newGrapeName = "";
                double newPercent = 0.0;

                foreach (var grape in oneLineGrapes)
                {
                    if (string.IsNullOrEmpty(grape) || string.IsNullOrWhiteSpace(grape)) continue;

                    var Grape = grape.Trim();
                    if (Grape.Last() == '%')
                    {
                        Grape = Grape.Substring(0, Grape.LastIndexOf(' ')).Trim();
                    }


                    if (char.IsDigit(Grape.First()))
                    {
                        string[] grapeWithPercent =
                            Grape.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                        var withoutPercentSign = grapeWithPercent[0].Remove(grapeWithPercent[0].Count() - 1, 1).Trim();

                        try
                        {
                            newPercent = double.Parse(withoutPercentSign);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("cannnot parse double " + e);
                        }

                        newGrapeName = grapeWithPercent[1];
                    }
                    else
                    {
                        newGrapeName = Grape;
                    }

                    //prevent duplicate
                    //if (grapes.Any(g => string.Compare(
                    //    g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8))).Replace(' ', '-'),
                    //    newGrapeName.Substring(0, ((int)(newGrapeName.Length * 0.8))).Replace(' ', '-'),
                    //    CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)) continue;

                    if (doCompare(grapes, newGrapeName)) continue;

                    grapes.Add(new Grape()
                    {
                        GrapeName = FirstLetterToUpperCase(newGrapeName.ToLower())
                    });
                }
            }


            return grapes;
        }

        private string fixTheLine(string line)
        {
            switch (line)
            {
                case "Sémillon 60%, sauvignon blanc 30%, muscadelle 10%":
                    return "60% Sémillon, 30% sauvignon blanc, 10% muscadelle";

                case "Corvina 60%, rondinella 20%, molinara 15%, övriga 5%":
                    return "60% Corvina, 20% rondinella, 15% molinara, 5% övriga";

                case "Tempranillo 95%, mazuelo 5%":
                    return " 95% Tempranillo, 5% mazuelo";

                case "Chardonnay 50%, pinot noir 30%, pinot meunier 20%":
                    return "50% Chardonnay, 30% pinot noir, 20% meunier";

                case "Parellada 40%, macabeo 35%, xarel-lo 25%":
                    return "40%  Parellada, 35% macabeo, 25% xarel-lo";

                case "Macabeo 50%, xarel-lo 40%, parellada 10%":
                    return "50% Macabeo, 40% xarel-lo, 10% parellada";


                default:
                    Console.WriteLine("something went wrong in fixTheLine function");
                    return null;
            }
        }

        #endregion


        private class grapeswithcount
        {
            public string Name { get; set; }
            public int Count { get; set; }
        }


        #region add wines

        private List<Wine> addWines(List<Artikel> systembolagetDataset, List<grapsfromjson> kaggleDataset,
            List<Grape> grapes, List<Country> countries, List<Region> regions, List<District> districts)
        {
            //Random rnd = new Random();
            var systembolagetsWines = addSystemWines(systembolagetDataset, grapes, countries, regions, districts);
            // var kaggleWines = addKaggleWines(kaggleDataset, grapes, countries, regions, districts);


            var wines = new List<Wine>(systembolagetsWines);
            //foreach (var wine in kaggleWines)
            //{
            //    if (wines.Any(w => string.Equals(w.Name, wine.Name, StringComparison.OrdinalIgnoreCase))) continue;
            //    wines.Add(wine);
            //}


            _databaseContext.Wines.AddRange(wines);
            _databaseContext.SaveChanges();

            return wines;
        }

        private List<Wine> addSystemWines(List<Artikel> systembolagetDataset, List<Grape> grapes,
            List<Country> countries, List<Region> regions, List<District> districts)
        {
            // var wineCounter = 0;
            List<Wine> wines = new List<Wine>();
            foreach (var sysWine in systembolagetDataset)
            {
                if (
                    sysWine.Varugrupp != "Rött vin" &&
                    sysWine.Varugrupp != "Vitt vin" &&
                    sysWine.Varugrupp != "Mousserande vin" &&
                    sysWine.Varugrupp != "Rosévin" &&
                    // wineJson.Varugrupp != "Blandl�dor vin" &&
                    sysWine.Varugrupp != "Röda - lägre alkoholhalt" &&
                    sysWine.Varugrupp != "Rosé - lägre alkoholhalt" &&
                    sysWine.Varugrupp != "Vita - lägre alkoholhalt"
                ) continue;


                var countryName = "Okänt ursprung";
                var regionName = "Okänt region";

                var districtName = "Okänt distrikt";
                var producer = "Okänt producent";
                var wineName = "";
                var alcohol = -1.0;
                var wineGrapes = new List<WineGrape>();

                if (!string.IsNullOrEmpty(sysWine.Ursprunglandnamn))
                    countryName = sysWine.Ursprunglandnamn;

                if (!string.IsNullOrEmpty(sysWine.Ursprung))
                    regionName = sysWine.Ursprung;

                if (!string.IsNullOrEmpty(sysWine.Producent))
                    producer = sysWine.Producent;

                if (sysWine.Alkoholhalt != null)
                    alcohol = Double.Parse(sysWine.Alkoholhalt.Remove(sysWine.Alkoholhalt.Length - 1, 1));

                //get the country, region and district from lists
                var country = countries.First(c =>
                    string.Equals(c.CountryName, countryName, StringComparison.OrdinalIgnoreCase));
                var region = regions.First(r => string.Equals(r.RegionName, regionName, StringComparison.OrdinalIgnoreCase)
                                                && r.CountryId == country.CountryId);
                var district = districts.First(d =>
                    string.Equals(d.DistrictName, districtName, StringComparison.OrdinalIgnoreCase)
                    && d.RegionId == region.RegionId);


                wineName = sysWine.Namn;
                if (!string.IsNullOrEmpty(sysWine.Namn2))
                    wineName = wineName + " " + sysWine.Namn2;

                if (wines.Any(w => string.Equals(w.Name, wineName, StringComparison.OrdinalIgnoreCase))) continue;


                wineGrapes = getSystemsWinesGrapes(sysWine.RavarorBeskrivning, grapes);

                var winetoadd = new Wine
                {
                    Name = wineName,
                    Producer = producer,
                    DistrictId = district.DistrictId,
                    Alcohol = alcohol,
                    WineGrapes = wineGrapes
                };
                wines.Add(winetoadd);
            }


            return wines;
        }

        private List<WineGrape> getSystemsWinesGrapes(string RavarorBeskrivning, List<Grape> grapes)
        {
            var wineGrapes = new List<WineGrape>();
            string[] spearator =
            {
                ",", " även ", " och en mindre del", " och ", "samt en liten del", "en liten del", " samt ",
                "Huvudsakligen", "Egendomen är planterad med", "Till största delen", "Vingården är planterad med"
            };

            if (string.IsNullOrEmpty(RavarorBeskrivning))
            {
                var g = grapes.First(g =>
                    string.Equals(g.GrapeName, "Övriga druvsorter", StringComparison.OrdinalIgnoreCase));
                wineGrapes.Add(new WineGrape
                {
                    Percent = 0,
                    Grape = g,
                    GrapeId = g.GrapeId
                });
                return wineGrapes;
            }

            var line = RavarorBeskrivning;

            if (line[line.Count() - 1] == '%')
                line = fixTheLine(line);

            if (line == null)
                Console.WriteLine();

            if (line[line.Count() - 1] == '.')
                line = line.Remove(line.Count() - 1, 1);

            if (line[0] == '.')
                line = line.Remove(0, 1);

            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ',')
                {
                    if (!char.IsDigit(line[i - 1])) continue;
                    if (!char.IsDigit(line[i + 1])) continue;
                    line = line.Remove(i, 1);
                    line = line.Insert(i, ".");
                }

                if (line[i] == '%')
                {
                    if (char.IsLetterOrDigit(line[i + 1]))
                        line = line.Insert(i + 1, " ");
                    if (char.IsWhiteSpace(line[i - 1]))
                        line = line.Remove(i - 1, 1);
                }
            }

            line = line.Replace(" and ", " och ");
            line = line.Replace("chardonnay.", "chardonnay,");
            line = line.Replace("Xarel-lo", "xarel.lo", StringComparison.OrdinalIgnoreCase);
            line = line.Replace("corvina 20% rondinella", "corvina, 20% rondinella");
            line = line.Replace("garnacha tinta (grenache) 25% syrah", "garnacha tinta (grenache), 25% syrah");

            var oneLineGrapes = line.Split(spearator, StringSplitOptions.None);


            var grapeName = "";
            double percent = 0.0;

            foreach (var grape in oneLineGrapes)
            {
                if (string.IsNullOrEmpty(grape) || string.IsNullOrWhiteSpace(grape)) continue;

                var Grape = grape.Trim();
                if (Grape.Last() == '%')
                {
                    Grape = Grape.Substring(0, Grape.LastIndexOf(' ')).Trim();
                }


                if (char.IsDigit(Grape.First()))
                {
                    string[] grapeWithPercent = Grape.Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                    var withoutPercentSign = grapeWithPercent[0].Remove(grapeWithPercent[0].Count() - 1, 1).Trim();

                    try
                    {
                        percent = double.Parse(withoutPercentSign);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("cannnot parse double " + e);
                    }

                    grapeName = grapeWithPercent[1];
                }
                else
                {
                    grapeName = Grape;
                }

                var grapeToAdd = grapes.First(g => string.Compare(
                    g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8)))
                        .Replace(' ', '-'),
                    grapeName.Substring(0, ((int)(grapeName.Length * 0.8)))
                        .Replace(' ', '-'),
                    CultureInfo.CurrentCulture,
                    CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0);


                wineGrapes.Add(new WineGrape
                {
                    Grape = grapeToAdd,
                    Percent = percent,
                    GrapeId = grapeToAdd.GrapeId
                });
            }

            return wineGrapes;
        }

        private List<Wine> addKaggleWines(List<grapsfromjson> kaggleDataset, List<Grape> grapes,
            List<Country> countries, List<Region> regions, List<District> districts)

        {
            List<Wine> wines = new List<Wine>();
            var lookuptable = LoadCountryLookUpTable();

            foreach (var kaggleWine in kaggleDataset)
            {
                var countryName = "Okänt ursprung";
                var regionName = "Okänt region";

                var districtName = "Okänt distrikt";
                var producer = "Okänt producent";
                var wineName = "";
                var alcohol = -1.0;
                var wineGrapes = new List<WineGrape>();


                if (!string.IsNullOrEmpty(kaggleWine.country))
                    countryName = kaggleWine.country;


                if (!string.IsNullOrEmpty(kaggleWine.region_2))
                    regionName = kaggleWine.region_2;
                else if (!string.IsNullOrEmpty(kaggleWine.province))
                    regionName = kaggleWine.province;

                if (!string.IsNullOrEmpty(kaggleWine.region_1))
                    districtName = kaggleWine.region_1;


                if (!string.IsNullOrEmpty(kaggleWine.winery))
                    producer = kaggleWine.winery;


                //get the country, region and district from lists
                var country = countries.First(co => string.Equals(co.CountryName,
                    (lookuptable.First(c => string.Equals(c.English, countryName, StringComparison.OrdinalIgnoreCase))
                        .Swedish), StringComparison.OrdinalIgnoreCase));

                var region = regions.First(r =>
                    string.Equals(r.RegionName, regionName, StringComparison.OrdinalIgnoreCase)
                    && r.CountryId == country.CountryId);
                var district = districts.First(d =>
                    string.Equals(d.DistrictName, districtName, StringComparison.OrdinalIgnoreCase)
                    && d.RegionId == region.RegionId);


                //get wine name 
                wineName = kaggleWine.title.Substring(0,
                    kaggleWine.title.IndexOfAny("0123456789".ToCharArray()) > 0
                        ? kaggleWine.title.IndexOfAny("0123456789".ToCharArray())
                        : (kaggleWine.title.IndexOf('(') > 0 ? kaggleWine.title.IndexOf('(') : kaggleWine.title.Length)
                ).Trim();
                wineGrapes = getKagglesWinegrapes(kaggleWine.variety, grapes);

                wines.Add(new Wine
                {
                    Name = wineName,
                    DistrictId = district.DistrictId,
                    Producer = producer,
                    Alcohol = alcohol,
                    WineGrapes = wineGrapes,
                });
            }

            return wines;
        }

        private List<WineGrape> getKagglesWinegrapes(string variety, List<Grape> grapes)
        {
            char[] spearator = { '-', ',' };
            var wineGrapes = new List<WineGrape>();
            //percent always -1 because there is no percent in the dataset 
            var percent = -1.0;

            if (string.IsNullOrEmpty(variety))
            {
                var g = grapes.First(g =>
                    string.Equals(g.GrapeName, "Övriga druvsorter", StringComparison.OrdinalIgnoreCase));
                wineGrapes.Add(new WineGrape
                {
                    Percent = -1,
                    Grape = g,
                    GrapeId = g.GrapeId
                });
                return wineGrapes;
            }

            var line = variety;
            line = line.Replace("Xarel-lo", "xarel.lo", StringComparison.OrdinalIgnoreCase);
            line = line.Replace("G-S-M", "G.S.M", StringComparison.OrdinalIgnoreCase);

            var grapesInLine = line.Split(spearator);

            foreach (var oldGrape in grapesInLine)
            {
                var grape = oldGrape.Trim();


                if (string.IsNullOrWhiteSpace(grape)) continue;
                //|| grapes.Any(g
                //=> string.Compare(
                //    g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8))).Replace(' ', '-'),
                //    grape.Substring(0, ((int)(grape.Length * 0.8))).Replace(' ', '-'),
                //    CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0)) continue;


                var grapeToAdd = grapes.First(g => string.Compare(
                    g.GrapeName.Substring(0, ((int)(g.GrapeName.Length * 0.8)))
                        .Replace(' ', '-'),
                    grape.Substring(0, ((int)(grape.Length * 0.8)))
                        .Replace(' ', '-'),
                    CultureInfo.CurrentCulture,
                    CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0);


                wineGrapes.Add(new WineGrape
                {
                    Grape = grapeToAdd,
                    Percent = percent,
                    GrapeId = grapeToAdd.GrapeId
                });
            }


            return wineGrapes;
        }

        #endregion


        #region add vintages

        private List<Vintage> addVintages(List<Artikel> systembolagetDataset, List<grapsfromjson> kaggleDataset,
            List<Wine> wines)
        {
            var systemVintages = addSystemVintages(systembolagetDataset, wines);
            // var kaggleVintages = addKaggleVintages(kaggleDataset, wines);


            var vintages = new List<Vintage>();
            vintages.AddRange(systemVintages);

            //foreach (var vintage in kaggleVintages)
            //{
            //    if (vintages.Any(v => v.WineId == vintage.WineId && v.Year == vintage.Year)) continue;
            //    vintages.Add(vintage);
            //}

            _databaseContext.Vintages.AddRange(vintages);
            _databaseContext.SaveChanges();


            return vintages;
        }

        private List<Vintage> addSystemVintages(List<Artikel> systemsDataset, List<Wine> wines)
        {
            var vintages = new List<Vintage>();

            foreach (var wine in systemsDataset)
            {
                if (
                    wine.Varugrupp != "Rött vin" &&
                    wine.Varugrupp != "Vitt vin" &&
                    wine.Varugrupp != "Mousserande vin" &&
                    wine.Varugrupp != "Rosévin" &&
                    // wineJson.Varugrupp != "Blandl�dor vin" &&
                    wine.Varugrupp != "Röda - lägre alkoholhalt" &&
                    wine.Varugrupp != "Rosé - lägre alkoholhalt" &&
                    wine.Varugrupp != "Vita - lägre alkoholhalt"
                ) continue;

                if (string.IsNullOrEmpty(wine.Argang)) continue;

                var wineName = wine.Namn;
                if (!string.IsNullOrEmpty(wine.Namn2))
                    wineName = wineName + " " + wine.Namn2;
                var theWine = wines.First(w => string.Equals(w.Name, wineName, StringComparison.OrdinalIgnoreCase));


                if (vintages.Any(v => v.WineId == theWine.WineId && v.Year == Int32.Parse(wine.Argang))) continue;
                vintages.Add(new Vintage
                {
                    Year = Int32.Parse(wine.Argang),
                    WineId = theWine.WineId
                });
            }

            return vintages;
        }

        private List<Vintage> addKaggleVintages(List<grapsfromjson> kaggleDataset, List<Wine> wines)
        {
            Regex _wordRegex = new Regex(@"\d{4}", RegexOptions.Compiled);
            //var counter = 0;

            var vintages = new List<Vintage>();
            foreach (var wine in kaggleDataset)
            {
                var year = 0;
                var wineName = "";


                var match = _wordRegex.Match(wine.title);

                if (string.IsNullOrEmpty(match.ToString())) continue;


                year = Int32.Parse(match.ToString());

                wineName = wine.title.Substring(0, wine.title.IndexOfAny("0123456789".ToCharArray())).Trim();

                if (string.IsNullOrEmpty(wineName)) continue;
                var theWine = wines.First(w => string.Equals(w.Name, wineName, StringComparison.OrdinalIgnoreCase));

                if (vintages.Any(v => v.WineId == theWine.WineId && v.Year == year)) continue;
                vintages.Add(new Vintage
                {
                    Year = year,
                    WineId = theWine.WineId
                });
            }

            return vintages;
        }

        #endregion
































        private string FirstLetterToUpperCase(string s)
        {
            if (string.IsNullOrEmpty(s))
                throw new ArgumentException("string is Empty");

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }
        //////////////////////
        //////////////////////
        //////////////////////
        //////////////////////
        ///load datasets
        public List<grapsfromjson> LoadKaggleDataset()
        {
            List<grapsfromjson> items = new List<grapsfromjson>();
            using (StreamReader r = new StreamReader(Environment.CurrentDirectory + "/wineDataset.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<grapsfromjson>>(json);
            }

            return items;
        }

        public List<countrylookuptable> LoadCountryLookUpTable()
        {
            List<countrylookuptable> items = new List<countrylookuptable>();
            using (StreamReader r = new StreamReader(Environment.CurrentDirectory + "/countryLookUpTable.json"))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<countrylookuptable>>(json);
            }

            return items;
        }


        public List<Artikel> LoadSystemBolagetDataset()
        {
            XmlDocument doc = new XmlDocument();
            var items = new syDataSetModel();
            using (StreamReader r = new StreamReader(Environment.CurrentDirectory + "/systembolaget.xml"))
            {
                string xml = r.ReadToEnd();
                doc.LoadXml(xml);
                var json = JsonConvert.SerializeXmlNode(doc);
                items = JsonConvert.DeserializeObject<syDataSetModel>(json);
            }

            return items.artiklar.artikel;
        }





        //////////////////////
        //////////////////////
        //////////////////////
        //////////////////////
        /// dataset models
        public class grapsfromjson
        {
            public string title { get; set; }
            public string country { get; set; }
            public string province { get; set; }

            public string variety { get; set; }
            public string winery { get; set; }
            public string designation { get; set; }
            public string region_1 { get; set; }
            public string region_2 { get; set; }
        }

        public class countrylookuptable
        {
            public string English { get; set; }
            public string Swedish { get; set; }
        }

        public class Artikel
        {
            //    public string nr { get; set; }
            //    public string Artikelid { get; set; }
            //   public string Varnummer { get; set; }
            //name1 + name2 
            public string Namn { get; set; }

            public string Namn2 { get; set; }

            //   public string Prisinklmoms { get; set; }
            //  public string Volymiml { get; set; }
            //  public string PrisPerLiter { get; set; }
            //   public string Saljstart { get; set; }
            //public string __invalid_name__Utg�tt { get; set; }
            //chosse vitt vin, r�tt vin ,rose och muserade
            public string Varugrupp { get; set; }

            //   public string Typ { get; set; }
            //   public string Stil { get; set; }
            //  public string Forpackning { get; set; }
            //  public string Forslutning { get; set; }
            //as region 
            public string Ursprung { get; set; }

            //as country
            public string Ursprunglandnamn { get; set; }

            //as Producent
            public string Producent { get; set; }

            //public string Leverantor { get; set; }
            //if exist as vintage
            public string Argang { get; set; }

            //public object Provadargang { get; set; }
            //alcohol
            public string Alkoholhalt { get; set; }

            //public string Sortiment { get; set; }
            // public string SortimentText { get; set; }
            //public string Ekologisk { get; set; }
            //public string Etiskt { get; set; }
            //public string Koscher { get; set; }
            //as grapes if exist
            public string RavarorBeskrivning { get; set; }
        }

        public class Artiklar
        {
            public List<Artikel> artikel { get; set; }
        }

        public class syDataSetModel
        {
            public Artiklar artiklar { get; set; }
        }


        public class yandexResponse
        {
            public int code { get; set; }
            public string lang { get; set; }
            public List<string> text { get; set; }
        }
    }
}


////////////////////////////////////
////////////////////////////////////
////////////////////////////////////
////////////////////////////////////
///Deprecated functions


////translate function ==> deprecated but maybe I'll use it in the future 
////use like 
////var translated  = await translate(toTranslate);
//private async Task<string> translate(string toTranslate)
//{
//    string translationResponse = "";
//    var apiKey = "trnsl.1.1.20200407T055005Z.99eb982eb5d35828.560fa657a7256df7f1064ea8edcf71818354aeca";
//    var languageCode = "sv";
//    var url = @"https://translate.yandex.net/api/v1.5/tr.json/translate?key=/APIKEY&text=/TEXT&lang=/LANGCODE";

//    url = url.Replace("/APIKEY", apiKey);
//    url = url.Replace("/TEXT", toTranslate);
//    url = url.Replace("/LANGCODE", languageCode);


//    var client = new HttpClient();
//    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
//    try
//    {
//        HttpResponseMessage response = await client.GetAsync(url);
//        response.EnsureSuccessStatusCode();

//        if (response != null)
//        {
//            var responseBody = JsonConvert.DeserializeObject<yandexResponse>(await response.Content.ReadAsStringAsync());
//            translationResponse = responseBody.text[0];

//        }

//    }
//    catch (HttpRequestException e)
//    {
//        throw new HttpRequestException(e.Message);
//    }


//    return translationResponse;
//}


//private static void CreatePasswordHash(string password, out byte[] passwordHash)
//{
//    var cost = 12;
//    var timeTarget = 10; // Milliseconds
//    string result;
//    long timeTaken;
//    do
//    {
//        var stopwatch = Stopwatch.StartNew();


//        result = BCrypt.Net.BCrypt.HashPassword(password, workFactor: cost);

//        stopwatch.Stop();
//        timeTaken = stopwatch.ElapsedMilliseconds;

//        cost -= 1;
//    } while ((timeTaken) >= timeTarget);


//    passwordHash = Encoding.ASCII.GetBytes(result);
//}