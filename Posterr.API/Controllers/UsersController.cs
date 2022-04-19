using Microsoft.AspNetCore.Mvc;
using Posterr.Services.Helpers;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Threading.Tasks;

namespace Posterr.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IFollowService _followService;

        public UsersController(IUserService userService, IFollowService followService)
        {
            _userService = userService;
            _followService = followService;
        }

        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> GetUserProfile(int userId)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<UserProfileModel> userProfileResponse = await _userService.GetUserProfile(userId, AuthenticatedUserId);

            if(!userProfileResponse.Success)
            {
                return BadRequest(userProfileResponse.Message);
            }

            return Ok(userProfileResponse.Data);
        }

        [HttpGet]
        [Route("follow/{userId}")]
        public IActionResult Follow(int userId)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse followUserResponse = _followService.FollowUser(userId, AuthenticatedUserId);
            if (!followUserResponse.Success)
            {
                return BadRequest(followUserResponse.Message);
            }

            return Ok();
        }

        [HttpGet]
        [Route("unfollow/{userId}")]
        public IActionResult UnfollowUser(int userId)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse unfollowUserResponse = _followService.UnfollowUser(userId, AuthenticatedUserId);
            if (!unfollowUserResponse.Success)
            {
                return BadRequest(unfollowUserResponse.Message);
            }

            return Ok();
        }
    }
}
