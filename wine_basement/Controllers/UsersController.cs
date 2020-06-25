using System.Threading.Tasks;
using basement.core.Models.UserModels;
using basement.infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace wine_basement.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody]AddUserModel model)
        {
            var isSuccess = await Task.Run(() => _userService.Create(model, this.User));

            if (!isSuccess)
                return BadRequest(new { messege = "Couldn not add the user" });

            return Ok(new { messege = "added user " + model.UserName + " successfully" });

        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserModel model)
        {
            var isSucess = await Task.Run(() => _userService.Update(model, this.User));

            if (!isSucess)
                return BadRequest(new { messege = "Something went wrong" });

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete()
        {
            var isSucess = await Task.Run(() => _userService.Delete(this.User));

            if (!isSucess)
                return BadRequest("Something went wrong, no identity recognized!");

            return Ok();
        }
    }
}