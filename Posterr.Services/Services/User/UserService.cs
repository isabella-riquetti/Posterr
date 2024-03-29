﻿using Posterr.Infra.Interfaces;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Collections.Generic;
using System.Linq;

namespace Posterr.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserTimelineService _userPostService;
        private readonly IFollowRepository _followRepository;

        public UserService(IUserRepository userRepository, IUserTimelineService userPostService, IFollowRepository followRepository)
        {
            _userPostService = userPostService;
            _followRepository = followRepository;
            _userRepository = userRepository;
        }

        public BaseResponse<UserProfileModel> GetUserProfile(int userId, int autheticatedUserId)
        {
            var userQuery = _userRepository.GetUser(userId);
            var response = userQuery
                .Select(u => new UserProfileModel
                {
                    UserId = u.Id,
                    CreatedAt = u.CreatedAt.ToString("MMMM dd, yyyy"),
                    Username = u.Username,
                    Followers = u.Following.Count(f => f.Unfollowed == false),
                    Following = u.Followers.Count(f => f.Unfollowed == false),
                    Posts = u.Posts.Count
                })
                .FirstOrDefault();

            if (response == null)
            {
                return BaseResponse<UserProfileModel>.CreateError("User not found");
            }

            BaseResponse<IList<PostResponseModel>> postsResponse = _userPostService.GetUserPosts(userId);
            if (!postsResponse.Success)
            {
                return BaseResponse<UserProfileModel>.CreateError(postsResponse.Message);
            }

            response.TopPosts = postsResponse.Data;
            response.Followed = _followRepository.IsUserFollowedBy(autheticatedUserId, userId);

            return BaseResponse<UserProfileModel>.CreateSuccess(response);
        }

        public BaseResponse UserExists(int id)
        {
            if (!_userRepository.UserExists(id))
            {
                return BaseResponse.CreateError("User not found");
            }

            return BaseResponse.CreateSuccess();
        }
    }
}
