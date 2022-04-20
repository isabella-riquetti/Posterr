using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using Posterr.Services.Model;
using Posterr.Services.Model.User;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posterr.Services
{
    public class UserService : IUserService
    {
        private readonly ApiContext _context;
        private readonly IPostService _postService;
        private readonly IFollowRepository _followRepository;


        public UserService(ApiContext context, IPostService postService, IFollowRepository followRepository)
        {
            _context = context;
            _postService = postService;
            _followRepository = followRepository;
        }

        public async Task<BaseResponse<UserProfileModel>> GetUserProfile(int userId, int autheticatedUserId)
        {
            var response = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileModel
                {
                    UserId = u.Id,
                    CreatedAt = u.CreatedAt.ToString("MMMM dd, yyyy"),
                    Username = u.Username,
                    Followers = u.Following.Count(f => f.Unfollowed == false),
                    Following = u.Followers.Count(f => f.Unfollowed == false),
                    Posts = u.Posts.Count()
                })
                .FirstOrDefaultAsync();

            if(response == null)
            {
                return BaseResponse<UserProfileModel>.CreateError("User not found");
            }

            BaseResponse<IList<PostResponseModel>> postsResponse = await _postService.GetUserPosts(userId);
            if (!postsResponse.Success)
            {
                return BaseResponse<UserProfileModel>.CreateError(postsResponse.Message);
            }
            
            response.TopPosts = postsResponse.Data;
            response.Followed = _followRepository.IsUserFollowedBy(autheticatedUserId, userId);

            return BaseResponse<UserProfileModel>.CreateSuccess(response);
        }

        public BaseResponse<int> CreateUser(CreateUserRequestModel request)
        {
            BaseResponse<int> userExist = _UserExists(request.Username);
            if (userExist.Success)
            {
                return BaseResponse<int>.CreateError("User already exists");
            }

            var user = new DB.Models.User()
            {
                Username = request.Username,
                Name = String.IsNullOrEmpty(request.Name) ? request.Username : request.Name,
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();

            BaseResponse<int> newUser = _UserExists(request.Username);
            if (!newUser.Success)
            {
                return BaseResponse<int>.CreateError("User could not be created");
            }

            return newUser;
        }

        public BaseResponse UserExists(int id)
        {
            bool response = _context.Users
                .Any(u => u.Id == id);

            if(!response)
            {
                return BaseResponse.CreateError("User not found");
            }
            
            return BaseResponse.CreateSuccess();
        }

        /// <summary>
        /// Check if the user exist by the username and return the id case exist
        /// </summary>
        /// <param name="username">Expected username</param>
        /// <returns>The userid in case exists</returns>
        private BaseResponse<int> _UserExists(string username)
        {
            int response = _context.Users
                .Where(u => u.Username == username)
                .Select(u => u.Id)
                .FirstOrDefault();

            if (response == 0)
            {
                return BaseResponse<int>.CreateError("User not found");
            }

            return BaseResponse<int>.CreateSuccess(response);
        }
    }
}
