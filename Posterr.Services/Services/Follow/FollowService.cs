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

        public BaseResponse FollowUser(int id, int authenticatedUserId)
        {
            if (IsUserFollowedBy(authenticatedUserId, id, out Follow follow))
            {
                return BaseResponse.CreateError("User is already followed by you");
            }

            // Case we don't have a follow yet, we should create one
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
            // Case we already have a followed with the Unfollowed true, we should change it to false
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

            return BaseResponse.CreateSuccess();
        }

        public BaseResponse UnfollowUser(int id, int authenticatedUserId)
        {
            if (!IsUserFollowedBy(authenticatedUserId, id, out Follow follow))
            {
                return BaseResponse.CreateError("You don't follow this user");
            }

            /*
             * UPDATE [Follows] SET [FollowerId] = @p0, [FollowingId] = @p1, [Unfollowed] = @p2
             * WHERE [Id] = @p3;
             * SELECT @@ROWCOUNT;
             */            
            follow.Unfollowed = true;
            _context.Follows.Update(follow);
            _context.SaveChanges();

            return BaseResponse.CreateSuccess();
        }

        public bool IsUserFollowedBy(int followerUserId, int followingUserId)
        {
            /* Query:
             */
            bool response = _context.Follows
                .Any(u => u.FollowerId == followerUserId && u.FollowingId == followingUserId && u.Unfollowed == false);

            return response;
        }

        /// <summary>
        /// Check if the FollowerUserId follows the FollowingUserId and return the Follow DB object in case it is followed
        /// </summary>
        /// <param name="followerUserId">The user ID that we test if it's following</param>
        /// <param name="followingUserId">The user ID that we test if it's followed</param>
        /// <param name="follow">The Follow object from the DB</param>
        /// <returns>If the FollowerUserId follows the FollowingUserId</returns>
        private bool IsUserFollowedBy(int followerUserId, int followingUserId, out Follow follow)
        {
            /* Query:
             * SELECT TOP(1) [f].[Id], [f].[FollowerId], [f].[FollowingId], [f].[Unfollowed]
             * FROM [Follows] AS [f]
             * WHERE ([f].[FollowerId] = @__follower_0) AND ([f].[FollowingId] = @__following_
             */
            follow = _context.Follows
                .FirstOrDefault(u => u.FollowerId == followerUserId && u.FollowingId == followingUserId);

            return follow != null && follow.Unfollowed == false;
        }
    }
}
