using Posterr.Services.Model;

namespace Posterr.Services.User
{
    public interface IFollowService
    {
        BaseResponse<string> FollowUser(int id, int authenticatedUserId);
        BaseResponse<string> UnfollowUser(int id, int authenticatedUserId);
        bool IsUserFollowedByAuthenticatedUser(int follower, int following);
    }
}
