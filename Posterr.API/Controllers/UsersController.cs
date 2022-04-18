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
        /* Ideal query:
        SELECT
        	[users].[Id],
        	[users].[CreatedAt],
        	[users].[Name],
        	[users].[Username],
        	count(distinct [posts].[Id]) as 'Posts',
        	count(distinct [followers].[Id]) as 'Followers',
        	count(distinct [following].[Id]) as 'Following'
        FROM [dbo].[Users] as [users]
        LEFT OUTER JOIN [Posts] AS [posts] ON [users].[Id] = [posts].[UserId]
        LEFT OUTER JOIN [Follows] AS [followers] ON [users].[Id] = [followers].[FollowerId]
        LEFT OUTER JOIN [Follows] AS [following] ON [users].[Id] = [following].[FollowingId]
        WHERE [users].[Id] = @__id_0
        GROUP BY [users].[Id], [users].[CreatedAt], [users].[Name], [users].[Username]
        */
        
        /* Current query:
         * Should fix that, is getting one line for each post and follower
        SELECT [t].[Id], [t].[CreatedAt], [t].[Name], [t].[Username], [p].[Id], [p].[Content], [p].[CreatedAt], [p].[OriginalPostId], [p].[UserId], [f].[Id], [f].[FollowerId], [f].[FollowingId], [f0].[Id], [f0].[FollowerId], [f0].[FollowingId]
        FROM (
            SELECT TOP(1) [u].[Id], [u].[CreatedAt], [u].[Name], [u].[Username]
            FROM [Users] AS [u]
            WHERE [u].[Id] = @__id_0
        ) AS [t]
        LEFT JOIN [Posts] AS [p] ON [t].[Id] = [p].[UserId]
        LEFT JOIN [Follows] AS [f] ON [t].[Id] = [f].[FollowerId]
        LEFT JOIN [Follows] AS [f0] ON [t].[Id] = [f0].[FollowingId]
        ORDER BY [t].[Id], [p].[Id], [f].[Id], [f0].[Id]
        */
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
                followers = user.Followers.Count,
                following = user.Following.Count,
                posts = user.Posts.Count
            };

            return Ok(response);
        }
    }
}
