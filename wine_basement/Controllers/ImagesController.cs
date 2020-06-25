using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace wine_basement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize]
    public class ImagesController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;

        public ImagesController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }

        [HttpGet("big")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[Produces(ImagesController)]
        public async Task<IActionResult> GetOriginal([FromQuery][Required] long wineId)
        {
            var result = await Task.Run(() => _infrastructureService.GetBigImage(wineId));
            if (result == null)
                return Ok(new { message = "Image not found" });

            return File(result, "image/jpeg");
        }


        [HttpGet("thumbnail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetThumbnail([FromQuery][Required] long wineId)
        {
            var result = await Task.Run(() => _infrastructureService.GetThumbnailImage(wineId));
            if (result == null)
                return Ok(new { message = "Image not found" });

            return File(result, "image/png");
        }
    }
}