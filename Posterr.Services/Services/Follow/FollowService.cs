using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Services.Model;
using Posterr.Services.User;
using System.Linq;

namespace Posterr.Services
{
    public class FollowService : IFollowService
    {
        private readonly ApiContext _context;

        public FollowService(ApiContext context)
        {
            _context = context;
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
                 * SET NOCOUNT ON;
                 * UPDATE [Follows] SET [FollowerId] = @p0, [FollowingId] = @p1, [Unfollowed] = @p2
                 * WHERE [Id] = @p3;
                 * SELECT @@ROWCOUNT;
                 */
                follow.Unfollowed = false;
                _context.Follows.Update(follow);
            }
            else
            {
                /* Query:
                 * SET NOCOUNT ON;
                 * INSERT INTO [Follows] ([FollowerId], [FollowingId], [Unfollowed])
                 * VALUES (@p0, @p1, @p2);
                 * SELECT [Id]
                 * FROM [Follows]
                 * WHERE @@ROWCOUNT = 1 AND [Id] = scope_identity();
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

            /*
             * UPDATE [Follows] SET [FollowerId] = @p0, [FollowingId] = @p1, [Unfollowed] = @p2
             * WHERE [Id] = @p3;
             * SELECT @@ROWCOUNT;
             */            
            follow.Unfollowed = true;
            _context.Follows.Update(follow);
            _context.SaveChanges();

            return BaseResponse<string>.CreateSuccess("You unfollowed this user");
        }

        public bool IsUserFollowedByAuthenticatedUser(int follower, int following)
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
             * SELECT TOP(1) [f].[Id], [f].[FollowerId], [f].[FollowingId], [f].[Unfollowed]
             * FROM [Follows] AS [f]
             * WHERE ([f].[FollowerId] = @__follower_0) AND ([f].[FollowingId] = @__following_
             */
            follow = _context.Follows
                .FirstOrDefault(u => u.FollowerId == follower && u.FollowingId == following);

            return follow != null && follow.Unfollowed == false;
        }
    }
}
