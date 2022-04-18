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

        public async Task<IActionResult> Get()
        {
            var users = await _context.Users
                                 .Include(u => u.Posts)
                                 .Include(u => u.Followers)
                                 .Include(u => u.Following)
                                 .ToArrayAsync();

            var response = users.Select(u => new
            {
                id = u.Id,
                nome = u.Name,
                username = u.Username,
                followers = u.Followers.Count(),
                following = u.Following.Count(),
                posts = u.Posts.Select(p => new
                {
                    id = p.Id,
                    content = p.Content,
                    createdAt = p.CreatedAt
                })
            });
            
            return Ok(response);
        }
    }
}
