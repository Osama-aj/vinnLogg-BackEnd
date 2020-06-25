using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WineListController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;


        public WineListController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }


        [HttpGet("~/api/userswinelist")]
        public async Task<IActionResult> GetUserWineList(
            // [FromQuery][Required] string userId ,
            [FromQuery] long shelfId = -1,
            [FromQuery] long wineId = -1,
            [FromQuery] long districtId = -1,
            [FromQuery] long regionId = -1,
            [FromQuery] long countryId = -1,
            [FromQuery] string startsWith = "",
            [FromQuery] long grapeId = -1)
        {
            var userWineList = await Task.Run(()
                => _infrastructureService.GetUsersWineList(this.User, shelfId, wineId, districtId, regionId,
                    countryId, startsWith, grapeId));

            return (userWineList == null) ? Ok(new { count = "empty" }) : Ok(userWineList);
        }

        [HttpGet("~/api/allwinelist")]
        public async Task<IActionResult> GetAllWineListAndUsersInfo(
            //   [FromQuery][Required] string userId,

            [FromQuery] long wineId = -1,
            [FromQuery] long districtId = -1,
            [FromQuery] long regionId = -1,
            [FromQuery] long countryId = -1,
            [FromQuery] string startsWith = "",
            [FromQuery] long grapeId = -1)
        {
            var wineListAndUsersInfo = await Task.Run(()
                => _infrastructureService.GetAllWineListWithUserInfo(this.User, wineId, districtId, regionId,
                    countryId, startsWith, grapeId));

            return (wineListAndUsersInfo == null) ? Ok(new { count = "empty" }) : Ok(wineListAndUsersInfo);
        }
    }
}