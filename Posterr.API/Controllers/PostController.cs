using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Posterr.Services.Helpers;
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
            if(!ValidationHelper.IsValidUserId(userId, out string errorMessage)
                || !ValidationHelper.IsSkipPossible(skip, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userProfileModel = await _postService.GetUserPosts(userId, skip ?? 0);

            if (!userProfileModel.Success)
            {
                return BadRequest(userProfileModel.Message);
            }

            return Ok(userProfileModel.Data);
        }
    }
}
