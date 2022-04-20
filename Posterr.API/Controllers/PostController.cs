using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Posterr.API.Helper;
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
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public PostController(IPostService postService, IUserService userService) : base()
        {
            _postService = postService;
            _userService = userService;
        }

        [HttpGet]
        [Route("byUser/{userId}/{skipPages?}")]
        public async Task<IActionResult> GetUserPosts(int userId, int skipPages = 0)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage)
                || !ValidationHelper.IsSkipPagePossible(skipPages, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = await _postService.GetUserPosts(userId, skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("timeline/following/{skipPages?}")]
        public async Task<IActionResult> GetUserTimeline(int skipPages = 0)
        {
            if (!ValidationHelper.IsSkipPagePossible(skipPages, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = await _postService.GetUserFollowingTimeline(AuthMockHelper.GetUserFromHeader(Request), skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("timeline/{skipPages?}")]
        public async Task<IActionResult> GetTimeline(int skipPages = 0)
        {
            if (!ValidationHelper.IsSkipPagePossible(skipPages, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = await _postService.GetTimeline(skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }
        
        [HttpGet]
        [Route("search/{text}/{skipPages?}")]
        public async Task<IActionResult> Search(string text, int skipPages = 0)
        {
            if (!ValidationHelper.IsSkipPagePossible(skipPages, out string errorMessage)
                || !ValidationHelper.IsValidPostContent(text, out errorMessage))
            {
                return BadRequest(errorMessage);
            }
            
            BaseResponse<IList<PostResponseModel>> userPostsResponse = await _postService.SearchByText(text, skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpPost]
        [Route("create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostRequestModel request)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ValidationHelper.IsValidPostContent(request?.Content, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<PostResponseModel> userPostsResponse = await _postService.CreatePost(request, AuthMockHelper.GetUserFromHeader(Request));

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }
    }
}
