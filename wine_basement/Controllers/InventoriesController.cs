using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using basement.core.Models.InventoryModels;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class InventoriesController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;

        public InventoriesController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddInventoryModel newInventory)
        {
            var result = await Task.Run(() => _infrastructureService.AddInventory(newInventory, this.User));

            if (result == null)
                return NotFound("Inventory is empty");

            return Ok(result);
        }


        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateInventoryModel updateInventory)
        {
            var result =
                await Task.Run(() => _infrastructureService.UpdateInventory(updateInventory, this.User));

            if (result == null)
                return NotFound("Inventory not found");

            return Ok(result);
        }


        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery] long id)
        {
            var isSuccess = await Task.Run(() => _infrastructureService.DeleteInventory(id, this.User));

            if (!isSuccess)
                return NotFound();
            return Ok();
        }
    }
}