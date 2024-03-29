﻿using Microsoft.AspNetCore.Mvc;
using Posterr.API.Helper;
using Posterr.Services.Helpers;
using Posterr.Services.Model;
using Posterr.Services.User;

namespace Posterr.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/users")]
    public class UsersController : ControllerBase
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
        public IActionResult GetUserProfile(int userId)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<UserProfileModel> userProfileResponse = _userService.GetUserProfile(userId, AuthMockHelper.GetUserFromHeader(Request));

            if (!userProfileResponse.Success)
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
            if (AuthMockHelper.GetUserFromHeader(Request) == userId)
            {
                return BadRequest("You can't follow yourself");
            }

            BaseResponse followUserResponse = _followService.FollowUser(userId, AuthMockHelper.GetUserFromHeader(Request));
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

            BaseResponse unfollowUserResponse = _followService.UnfollowUser(userId, AuthMockHelper.GetUserFromHeader(Request));
            if (!unfollowUserResponse.Success)
            {
                return BadRequest(unfollowUserResponse.Message);
            }

            return Ok();
        }
    }
}
