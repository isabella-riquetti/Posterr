using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Services.Model;
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
        private readonly IFollowService _followService;

        public UserService(ApiContext context, IPostService postService, IFollowService followService)
        {
            _context = context;
            _postService = postService;
            _followService = followService;
        }

        public async Task<BaseResponse<UserProfileModel>> GetUserProfile(int id, int autheticatedUserId)
        {
            /* Query:
             * SELECT TOP(1) [u].[Id], [u].[CreatedAt], [u].[Username], (
             *     SELECT COUNT(*)
             *     FROM [Follows] AS [f]
             *     WHERE ([u].[Id] = [f].[FollowingId]) AND ([f].[Unfollowed] <> CAST(1 AS bit))), (
             *     SELECT COUNT(*)
             *     FROM [Follows] AS [f0]
             *     WHERE ([u].[Id] = [f0].[FollowerId]) AND ([f0].[Unfollowed] <> CAST(1 AS bit))), (
             *     SELECT COUNT(*)
             *     FROM [Posts] AS [p]
             *     WHERE [u].[Id] = [p].[UserId])
             * FROM [Users] AS [u]
             * WHERE [u].[Id] = @__id_0
            */
            var response = await _context.Users
                .Where(u => u.Id == id)
                .Select(u => new UserProfileModel
                {
                    Id = u.Id,
                    CreatedAt = u.CreatedAt.ToString("MMMM dd, yyyy"),
                    Username = u.Username,
                    Followers = u.Following.Count(f => f.Unfollowed == false),
                    Following = u.Followers.Count(f => f.Unfollowed == false),
                    Posts = u.Posts.Count()
                })
                .FirstOrDefaultAsync();

            if(response == null)
            {
                return BaseResponse<UserProfileModel>.CreateFailure("User not found");
            }

            BaseResponse<IList<PostResponseModel>> postsResponse = await _postService.GetUserPosts(id);
            if (!postsResponse.Success)
            {
                return BaseResponse<UserProfileModel>.CreateFailure(postsResponse.Message);
            }
            
            response.TopPosts = postsResponse.Data;
            response.Followed = _followService.IsUserFollowedByAuthenticatedUser(autheticatedUserId, id);

            return BaseResponse<UserProfileModel>.CreateSuccess(response);
        }

        public Task<BaseResponse<UserProfileModel>> GetUserProfile(int userId, object authenticatedUserId)
        {
            throw new NotImplementedException();
        }
    }
}
