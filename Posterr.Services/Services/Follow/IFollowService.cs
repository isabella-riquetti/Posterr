using Posterr.Services.Model;

namespace Posterr.Services.User
{
    public interface IFollowService
    {
        BaseResponse FollowUser(int id, int authenticatedUserId);
        BaseResponse UnfollowUser(int id, int authenticatedUserId);
        bool IsUserFollowedByAuthenticatedUser(int follower, int following);
    }
}
