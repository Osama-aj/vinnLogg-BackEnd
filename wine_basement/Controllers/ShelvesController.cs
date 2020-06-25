using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using basement.core.Models.ShelfModels;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ShelvesController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;

        public ShelvesController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await Task.Run(() => _infrastructureService.GetAllShelves(this.User));

            //if (result == null)
            //    return NotFound();

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddShelfModel newShelves)
        {
            var result = await Task.Run(() => _infrastructureService.AddShelf(newShelves, this.User));

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateShelfModel updateShelf)
        {

            var result = await Task.Run(() => _infrastructureService.UpdateShelf(updateShelf, this.User));

            if (result == null)
                return BadRequest("something went wrong!!");

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery][Required] long shelfId)
        {
            var isSuccess = await Task.Run(() => _infrastructureService.DeleteShelf(shelfId, this.User));

            if (!isSuccess)
                return NotFound();
            return Ok();
        }
    }
}