using System.Threading.Tasks;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;
using basement.core.Models.WineModels;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class WinesController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;


        public WinesController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }


        [HttpGet]
        public async Task<IActionResult> Get(
            [FromQuery] string startsWith = "",
            [FromQuery] long wineId = -1,
            [FromQuery] long districtId = -1,
            [FromQuery] long regionId = -1,
            [FromQuery] long countryId = -1,
            [FromQuery] long grapeId = -1)
        {
            var wines =
                await Task.Run(() =>
                    _infrastructureService.GetAllWinesOrBy(wineId, districtId, regionId, countryId, startsWith,
                        grapeId));
            return Ok(wines);
        }


        /// <summary>
        /// add new wine // post 
        /// </summary>
        /// <param name="newWine"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddWineModel newWine)
        {
            var result = await Task.Run(() => _infrastructureService.AddWine(newWine));

            if (result == null)
                return BadRequest("something went wrong!!");

            return Ok(result);
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateWineModel updatedWine)
        {
            var result = await Task.Run(() => _infrastructureService.UpdateWine(updatedWine));

            if (result == null)
                return BadRequest("something went wrong!!");

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] [Required] long id)
        {
            var isSuccess = await Task.Run(() => _infrastructureService.DeleteWine(id));

            if (!isSuccess)
                return NotFound();
            return Ok();
        }
    }
}