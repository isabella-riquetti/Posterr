using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Posterr.Services.Helpers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Threading.Tasks;

namespace Posterr.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ValidationHelper.IsValidUserId(id, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }
            
            BaseResponse<UserProfileModel> response = await _userService.GetUserProfile(id, AuthenticatedUserId);

            if(!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }
    }
}
