using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.Services.Model;
using Posterr.Services.User;
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

        public async Task<UserProfileModel> GetUserProfile(int id, int autheticatedUserId)
        {
            /* Query:
            * SELECT TOP(1) [u].[Id], CONVERT(VARCHAR(100), [u].[CreatedAt]) AS [CreatedAt], [u].[Username], (
            *     SELECT COUNT(*)
            *     FROM [Follows] AS [f]
            *     WHERE [u].[Id] = [f].[FollowerId]) AS [Followers], (
            *     SELECT COUNT(*)
            *     FROM [Follows] AS [f0]
            *     WHERE [u].[Id] = [f0].[FollowingId]) AS [Following], (
            *     SELECT COUNT(*)
            *     FROM [Posts] AS [p]
            *     WHERE [u].[Id] = [p].[UserId]) AS [Posts]
            * FROM [Users] AS [u]
            * WHERE [u].[Id] = @__id_0
            */
            var response = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .Where(u => u.Id == id)
                .Select(u => new UserProfileModel
                {
                    Id = u.Id,
                    CreatedAt = u.CreatedAt.ToString("MMMM dd, yyyy"),
                    Username = u.Username,
                    Followers = u.Followers.Count(),
                    Following = u.Following.Count(),
                    Posts = u.Posts.Count()
                })
                .FirstOrDefaultAsync();

            response.Followed = await IsUserFollowedByAuthenticatedUser(id, autheticatedUserId);
            response.TopPosts = await _postService.GetUserPosts(id);

            return response;
        }


        private async Task<bool> IsUserFollowedByAuthenticatedUser(int id, int autheticatedUserId)
        {
            /* Query:
             * SELECT CASE
             *     WHEN EXISTS (
             *         SELECT 1
             *         FROM [Follows] AS [f]
             *         WHERE ([f].[FollowerId] = @__id_0) AND ([f].[FollowingId] = @__autheticatedUserId_1)) THEN CAST(1 AS bit)
             *     ELSE CAST(0 AS bit)
             * END
             */
            bool response = await _context.Follows
                .AnyAsync(u => u.FollowerId == id && u.FollowingId == autheticatedUserId);

            return response;
        }
    }
}
