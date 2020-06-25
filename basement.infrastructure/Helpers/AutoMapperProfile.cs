using AutoMapper;
using basement.core.Entities.GeneralSection;
using basement.core.Entities.UserSection;
using basement.core.Models;
using basement.core.Models.AddMetadata;
using basement.core.Models.GeneralSectionDto;
using basement.core.Models.GradeModels;
using basement.core.Models.InventoryModels;
using basement.core.Models.ShelfModels;
using basement.core.Models.UserModels;
using basement.core.Models.UserSectionDto;
using basement.core.Models.VintageModels;
using basement.core.Models.WineModels;

namespace basement.infrastructure.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Country, CountryDto>();
            CreateMap<Country, WinelistCountryDto>();
            CreateMap<AddCountryModel, Country>();

            CreateMap<Region, RegionDto>();
            CreateMap<Region, WinelistRegionDto>();
            CreateMap<AddRegionModel, Region>();

            CreateMap<District, DistrictDto>();
            CreateMap<AddDistrictModel, District>();


            CreateMap<Grape, GrapeDto>();
            CreateMap<WineGrape, WineGrapeDto>()
                .ForMember(des => des.GrapeName,
                    opt
                        => opt.MapFrom(g
                            => g.Grape.GrapeName));


            CreateMap<Wine, WineDto>();
            CreateMap<AddWineModel, Wine>();


            CreateMap<UpdateWineModel, Wine>()
                .ForMember(dest => dest.Alcohol, opt
                    => opt.Condition(src => (src.Alcohol != 0)))
                .ForMember(dest => dest.DistrictId, opt
                    => opt.Condition(src => (src.DistrictId != 0)))
                .ForAllOtherMembers(opts
                    => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Inventory, InventoryDto>();
            CreateMap<AddInventoryModel, Inventory>();
            CreateMap<UpdateInventoryModel, Inventory>()
                .ForMember(dest => dest.ShelfId, opt
                    => opt.Condition(src => (src.ShelfId != null)))
                 .ForMember(dest => dest.Amount, opt
                    => opt.Condition(src => (src.Amount != null)))
                .ForAllOtherMembers(opts
                    => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Shelf, ShelfDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ShelfName));
            CreateMap<AddShelfModel, Shelf>();
            CreateMap<UpdateShelfModel, Shelf>()
                .ForAllOtherMembers(opts
                    => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<Vintage, VintageDto>();
            CreateMap<AddVintageModel, Vintage>();
            CreateMap<UpdateVintageModel, Vintage>()
                .ForAllOtherMembers(opts
                    => opts.Condition((src, dest, srcMember) => srcMember != null));


            //user management
            CreateMap<AddUserModel, User>();


            CreateMap<User, UserDto>();

            CreateMap<UpdateUserModel, User>();
            //.ForAllOtherMembers(opts
            //    => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<AddGradeModel, GradeTable>();
            CreateMap<UpdateGradeModel, GradeTable>()
                .ForMember(dest => dest.Grade, opt
                    => opt.Condition(src => (src.Grade != 0)))
                .ForAllOtherMembers(opts
                    => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<GradeTable, GradeDto>();
        }
    }
}