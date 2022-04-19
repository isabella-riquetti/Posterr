using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Posterr.DB;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [Route("{requestId}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest("The ID should be a number between 1 and 2147483647");
            }

            UserProfileModel userProfileModel = await _userService.GetUserProfile(id, AuthenticatedUserId);

            if (userProfileModel == null)
            {
                return NotFound("User not found");
            }

            return Ok(userProfileModel);
        }
    }
}
