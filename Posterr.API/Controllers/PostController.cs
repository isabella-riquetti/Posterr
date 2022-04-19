using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Posterr.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/postS")]
    public class PostController : BaseController
    {
        private readonly IPostService _postService;
        private readonly ILogger<PostController> _logger;

        public PostController(IPostService postService, ILogger<PostController> logger)
        {
            _logger = logger;
            _postService = postService;
        }

        [HttpGet]
        [Route("byUser/{userId}/{skip?}")]
        public async Task<IActionResult> Get(int userId, int? skip)
        {
            if (userId <= 0)
            {
                return BadRequest("The User ID should be a number between 1 and 2147483647");
            }
            if (skip < 0)
            {
                return BadRequest("Can't skip negative number of posts");
            }

            IList<PostResponseModel> userProfileModel = await _postService.GetUserPosts(userId, skip ?? 0);

            if (userProfileModel == null)
            {
                return NotFound("User not found");
            }

            return Ok(userProfileModel);
        }
    }
}
