using Microsoft.AspNetCore.Mvc;
using Posterr.API.Helper;
using Posterr.Services.Helpers;
using Posterr.Services.Model;
using Posterr.Services.User;

namespace Posterr.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/posts")]
    public class PostController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
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
