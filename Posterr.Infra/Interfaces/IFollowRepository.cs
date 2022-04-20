using Posterr.DB.Models;

namespace Posterr.Infra.Interfaces
{
    public interface IFollowRepository
    {
        /// <summary>
        /// Update a Follow Unfollowed status
        /// </summary>
        /// <param name="follow">The Follow that should be updates</param>
        /// <param name="follow">The new unfollow status</param>
        void UpdateUnfollowedStatus(Follow follow, bool unfollow);

        /// <summary>
        /// Create a new Follow object in the DB
        /// </summary>
        /// <param name="followerUserId">The user that is going to start following</param>
        /// <param name="followingUserId">The user that is going to be followed</param>
        void CreateFollow(int followerUserId, int followingUserId);

        /// <summary>
        /// Check if the FollowerUserId follows the FollowingUserId
        /// </summary>
        /// <param name="followerUserId">The user ID that we test if it's following</param>
        /// <param name="followingUserId">The user ID that we test if it's followed</param>
        /// <returns>If it is followed</returns>
        bool IsUserFollowedBy(int followerUserId, int followingUserId);

        /// <summary>
        /// Check if the FollowerUserId follows the FollowingUserId and return the Follow DB object in case it is followed
        /// </summary>
        /// <param name="followerUserId">The user ID that we test if it's following</param>
        /// <param name="followingUserId">The user ID that we test if it's followed</param>
        /// <param name="follow">The Follow object from the DB</param>
        /// <returns>If the FollowerUserId follows the FollowingUserId</returns>
        bool IsUserFollowedBy(int followerUserId, int followingUserId, out Follow follow);
    }
}

