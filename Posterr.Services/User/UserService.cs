using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posterr.Services
{
    public class UserService : IUserService
    {
        private readonly ApiContext _context;
        private readonly IPostService _postService;

        public UserService(ApiContext context, IPostService postService)
        {
            _context = context;
            _postService = postService;
        }

        public BaseResponse<string> FollowUser(int id, int authenticatedUserId)
        {
            if (IsUserFollowedByAuthenticatedUser(authenticatedUserId, id, out Follow follow))
            {
                return BaseResponse<string>.CreateFailure("User is already followed by you");
            }

            if (follow != null)
            {
                /* Query:
                */
                follow.Unfollowed = false;
                _context.Follows.Update(follow);
            }
            else
            {
                /* Query:
                */
                Follow newFollow = new Follow()
                {
                    FollowerId = authenticatedUserId,
                    FollowingId = id,
                    Unfollowed = false
                };
                _context.Follows.Add(newFollow);
            }
            _context.SaveChanges();

            return BaseResponse<string>.CreateSuccess("You now follow this user");
        }

        public BaseResponse<string> UnfollowUser(int id, int authenticatedUserId)
        {
            if (!IsUserFollowedByAuthenticatedUser(authenticatedUserId, id, out Follow follow))
            {
                return BaseResponse<string>.CreateFailure("You don't follow this user");
            }

            /* Query:
            */
            follow.Unfollowed = true;
            _context.Follows.Update(follow);
            _context.SaveChanges();

            return BaseResponse<string>.CreateSuccess("You unfollowed this user");
        }

        public async Task<BaseResponse<UserProfileModel>> GetUserProfile(int id, int autheticatedUserId)
        {
            /* Query:
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
            response.Followed = IsUserFollowedByAuthenticatedUser(autheticatedUserId, id);

            return BaseResponse<UserProfileModel>.CreateSuccess(response);
        }


        private bool IsUserFollowedByAuthenticatedUser(int follower, int following)
        {
            /* Query:
             */
            bool response = _context.Follows
                .Any(u => u.FollowerId == follower && u.FollowingId == following && u.Unfollowed == false);

            return response;
        }

        private bool IsUserFollowedByAuthenticatedUser(int follower, int following, out Follow follow)
        {
            /* Query:
             */
            follow = _context.Follows
                .FirstOrDefault(u => u.FollowerId == follower && u.FollowingId == following);

            return follow != null && follow.Unfollowed == false;
        }
    }
}
