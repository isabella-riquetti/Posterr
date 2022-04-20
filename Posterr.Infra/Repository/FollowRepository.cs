using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using System.Linq;

namespace Posterr.Infra.Repository
{
    public class FollowRepository : IFollowRepository
    {
        private readonly ApiContext _context;

        public FollowRepository(ApiContext context)
        {
            _context = context;
        }

        public void UpdateUnfollowedStatus(Follow follow, bool unfollow)
        {
            follow.Unfollowed = unfollow;
            _context.Follows.Update(follow);
            _context.SaveChanges();
        }

        public void CreateFollow(int followerUserId, int followingUserId)
        {
            Follow newFollow = new Follow()
            {
                FollowerId = followerUserId,
                FollowingId = followingUserId,
                Unfollowed = false
            };
            _context.Follows.Add(newFollow);
            _context.SaveChanges();
        }

        public bool IsUserFollowedBy(int followerUserId, int followingUserId)
        {
            bool response = _context.Follows
                .Any(u => u.FollowerId == followerUserId && u.FollowingId == followingUserId && u.Unfollowed == false);

            return response;
        }

        public bool IsUserFollowedBy(int followerUserId, int followingUserId, out Follow follow)
        {
            follow = _context.Follows
                .FirstOrDefault(u => u.FollowerId == followerUserId && u.FollowingId == followingUserId);

            return follow != null && follow.Unfollowed == false;
        }
    }
}

