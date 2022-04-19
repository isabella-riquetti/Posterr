using Posterr.Services.Model;
using System.Threading.Tasks;

namespace Posterr.Services.User
{
    public interface IUserService
    {
        /// <summary>
        /// Get the user profile info and last 5 posts
        /// </summary>
        /// <param name="userId">User that should be unfollowed</param>
        /// <param name="authenticatedUserId">The current authenticated, is used to check to check whether the user is followed by it</param>
        /// <returns>The profile ingo and last 5 posts</returns>
        public Task<BaseResponse<UserProfileModel>> GetUserProfile(int userId, int autheticatedUserId);
        BaseResponse UserExists(int id);
    }
}
