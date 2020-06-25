using System;
using System.Security.Claims;
using System.Threading.Tasks;
using basement.core.Models.AddMetadata;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MetaDataController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;

        public MetaDataController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }
        [HttpPost("countries")]
        public async Task<IActionResult> PostCountry([FromBody]AddCountryModel newCountry)
        {
            var result = await Task.Run(() => _infrastructureService.AddCountry(newCountry));

            if (result == null)
                return BadRequest("something went wrong!!");

            return Ok(result);
        }
        [HttpPost("regions")]
        public async Task<IActionResult> PostRegion([FromBody]AddRegionModel newRegion)
        {
            var result = await Task.Run(() => _infrastructureService.AddRegion(newRegion));

            if (result == null)
                return BadRequest("something went wrong!!");

            return Ok(result);
        }
        [HttpPost("districts")]
        public async Task<IActionResult> PostDistrict([FromBody]AddDistrictModel newDistrict)
        {
            var result = await Task.Run(() => _infrastructureService.AddDistrict(newDistrict));

            if (result == null)
                return BadRequest("something went wrong!!");

            return Ok(result);
        }
        [HttpGet]
        public async Task<IActionResult> GetMetaData()
        {

            var metadata = await Task.Run(() => _infrastructureService.GetMetaData());
            //if (metadata == null)
            //{
            //    return NotFound();
            //}

            return Ok(metadata);
        }

        [HttpGet("countries")]
        public async Task<IActionResult> GetCountries()
        {
            var countries = await Task.Run(() => _infrastructureService.GetAllCountriesRegionsDistricts());
            //if (countries == null)
            //{
            //    return NotFound();
            //}

            return Ok(countries);
        }

        [HttpGet("grapes")]
        public async Task<IActionResult> GetGrapes()
        {
            var grapes = await Task.Run(() => _infrastructureService.GetAllGrapes());
            //if (grapes == null)
            //{
            //    return NotFound();
            //}


            return Ok(grapes);
        }

        [AllowAnonymous]
        [HttpPost("mock")]
        public async Task<IActionResult> MockData()
        {

            await Task.Run(() => _infrastructureService.Mock());
            return Ok();
        }
    }
}