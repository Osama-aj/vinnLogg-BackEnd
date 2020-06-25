using basement.core.Models.GradeModels;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace wine_basement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GradesController : ControllerBase
    {
        private readonly IInfrastructureService _infrastructureService;

        public GradesController(IInfrastructureService infrastructureService)
        {
            _infrastructureService = infrastructureService;
        }

        [Consumes("application/json")]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddGradeModel newGrade)
        {
            var result = await Task.Run(() => _infrastructureService.AddGrade(newGrade, this.User));

            if (result == null)
                return Ok(new { message = "Inventory not found" });

            return Ok(result);
        }

        [Consumes("application/json")]
        [HttpPut]
        public async Task<IActionResult> Put([FromBody] UpdateGradeModel updateGrade)
        {
            var GradeDto = await Task.Run(() => _infrastructureService.UpdateGrade(updateGrade, this.User));

            if (GradeDto == null)
                return Ok(new { count = 0, message = "not found or something went wrong " });
            return Ok(GradeDto);
        }
    }
}