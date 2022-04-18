using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Posterr.DB;
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
        private readonly ApiContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ApiContext context, ILogger<UsersController> logger)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("{requestId}")]
        // TODO: Move query to a separate class or repository
        // TODO: Improve query to get only the count
        public async Task<IActionResult> Get(string requestId)
        {
            if (!int.TryParse(requestId, out int id) || id <= 0)
            {
                return BadRequest("The ID should be a number between 1 and 2147483647");
            }           

            var user = await _context.Users
                .Where(u => u.Id == id)
                                 .Include(u => u.Posts)
                                 .Include(u => u.Followers)
                                 .Include(u => u.Following)
                                 .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            var response = new
            {
                id = user.Id,
                nome = user.Name,
                createdAt = user.CreatedAt,
                username = user.Username,
                followers = user.Followers.Count(),
                following = user.Following.Count(),
                posts = user.Posts.Count()
            };
            
            return Ok(response);
        }
    }
}
