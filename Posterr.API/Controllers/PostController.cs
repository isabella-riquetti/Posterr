using Microsoft.AspNetCore.Mvc;
using Posterr.API.Helper;
using Posterr.Services.Helpers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Collections.Generic;

namespace Posterr.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/posts")]
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
        [Route("user/{userId}/")]
        public IActionResult GetUserPosts(int userId, int skipPages = 0)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage)
                || !ValidationHelper.IsValuePositiveOrNeutral(skipPages, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _postService.GetUserPosts(userId, skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("timeline/following/")]
        public IActionResult GetUserTimeline(int skipPages = 0)
        {
            if (!ValidationHelper.IsValuePositiveOrNeutral(skipPages, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _postService.GetUserFollowingTimeline(AuthMockHelper.GetUserFromHeader(Request), skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("timeline/")]
        public IActionResult GetTimeline(int skipPages = 0)
        {
            if (!ValidationHelper.IsValuePositiveOrNeutral(skipPages, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _postService.GetTimeline(skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("search/{text}/")]
        public IActionResult Search(string text, int skipPages = 0)
        {
            if (!ValidationHelper.IsValuePositiveOrNeutral(skipPages, out string errorMessage)
                || !ValidationHelper.IsValidContentLength(text, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _postService.SearchByText(text, skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpPost]
        [Route("create")]
        public IActionResult CreatePost([FromBody] CreatePostRequestModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ValidationHelper.IsValidPostRequest(request, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<PostResponseModel> userPostsResponse = _postService.CreatePost(request, AuthMockHelper.GetUserFromHeader(Request));

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }
    }
}
