﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, IFollowService followService, ILogger<UsersController> logger)
        {
            _logger = logger;
            _userService = userService;
            _followService = followService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!ValidationHelper.IsValidUser(id, _userService.UserExist, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<UserProfileModel> response = await _userService.GetUserProfile(id, AuthenticatedUserId);

            if(!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Data);
        }

        [HttpGet]
        [Route("follow/{id}")]
        public IActionResult Follow(int id)
        {
            if (!ValidationHelper.IsValidUser(id, _userService.UserExist, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse response = _followService.FollowUser(id, AuthenticatedUserId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Message);
        }

        [HttpGet]
        [Route("unfollow/{id}")]
        public IActionResult Unfollow(int id)
        {
            if (!ValidationHelper.IsValidUser(id, _userService.UserExist, out string errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse response = _followService.UnfollowUser(id, AuthenticatedUserId);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response.Message);
        }
    }
}
