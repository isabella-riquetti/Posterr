﻿using Microsoft.AspNetCore.Mvc;
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
        [Route("byUser/{userId}/{skipPages?}")]
        public async Task<IActionResult> GetUserPosts(int userId, int? skipPages)
        {
            if (!ValidationHelper.IsValidUser(userId, _userService.UserExists, out string errorMessage)
                || !ValidationHelper.IsSkipPagePossible(skipPages, out errorMessage))
            {
                return BadRequest(errorMessage);
            }

            BaseResponse<IList<PostResponseModel>> userPostsResponse = await _postService.GetUserPosts(userId, skipPages ?? 0);

            if (!userPostsResponse.Success)
            {
                return BadRequest(userPostsResponse.Message);
            }

            return Ok(userPostsResponse.Data);
        }
    }
}
