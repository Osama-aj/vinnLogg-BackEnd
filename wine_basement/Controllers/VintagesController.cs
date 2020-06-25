using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using basement.core.Models.VintageModels;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ControllerBase = Microsoft.AspNetCore.Mvc.ControllerBase;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class VintagesController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;

        public VintagesController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }


        [HttpGet]
        public async Task<IActionResult> Get([FromQuery][Required] long wineId)
        {
            var vintages = await Task.Run(() => _infrastructureService.GetAllVintagesByWineId(wineId));
            //if (vintages == null)
            //    return NotFound();
            return Ok(vintages);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddVintageModel newVintage)
        {
            var result = await Task.Run(() => _infrastructureService.AddVintage(newVintage));

            if (result == null)
                return Ok(new { message = "something went wrong!!" });

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateVintageModel newVintage)
        {
            var result = await Task.Run(() => _infrastructureService.UpdateVintage(newVintage));

            if (result == null)
                return Ok(new { message = "something went wrong!!" });

            return Ok(result);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] long id)
        {
            var isSuccess = await Task.Run(() => _infrastructureService.DeleteVintage(id));

            if (!isSuccess)
                return Ok(new { message = "something went wrong!!" });
            return Ok();
        }
    }
}