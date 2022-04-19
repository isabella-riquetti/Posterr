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
        private readonly IUserService _userService;

        public PostController(IPostService postService, IUserService userService)
        {
            _postService = postService;
            _userService = userService;
        }

        [HttpGet]
        [Route("byUser/{userId}/{skip?}")]
        public async Task<IActionResult> Get(int userId, int? skip)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExist, out string errorMessage)
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
