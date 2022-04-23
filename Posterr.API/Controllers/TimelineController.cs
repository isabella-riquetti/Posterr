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
    [Route("api/timeline")]
    public class TimelineController : ControllerBase
    {
        private readonly ITimelineService _timelineService;
        private readonly IUserTimelineService _userTimelineService;
        private readonly IUserService _userService;

        public TimelineController(ITimelineService timelineService, IUserTimelineService userTimelineService, IUserService userService)
        {
            _timelineService = timelineService;
            _userTimelineService = userTimelineService;
            _userService = userService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult GetTimeline(int skipPages = 0)
        {
            if (!ValidationHelper.IsValuePositiveOrNeutral(skipPages, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _timelineService.GetTimeline(skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("following")]
        public IActionResult GetUserTimeline(int skipPages = 0)
        {
            if (!ValidationHelper.IsValuePositiveOrNeutral(skipPages, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _timelineService.GetUserFollowingTimeline(AuthMockHelper.GetUserFromHeader(Request), skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("user/{userId}")]
        public IActionResult GetUserPosts(int userId, int skipPages = 0)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage)
                || !ValidationHelper.IsValuePositiveOrNeutral(skipPages, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _userTimelineService.GetUserPosts(userId, skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }

        [HttpGet]
        [Route("search")]
        public IActionResult Search(string text, int skipPages = 0)
        {
            if (!ValidationHelper.IsValuePositiveOrNeutral(skipPages, out string errorMessage)
                || !ValidationHelper.IsValidContentLength(text, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = _timelineService.SearchByText(text, skipPages);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }
    }
}
