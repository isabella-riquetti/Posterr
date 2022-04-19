using Posterr.Services.Model;
using System.Threading.Tasks;

namespace Posterr.Services.User
{
    public interface IUserService
    {
        public Task<BaseResponse<UserProfileModel>> GetUserProfile(int id, int autheticatedUserId);
        BaseResponse<string> FollowUser(int id, int authenticatedUserId);
        BaseResponse<string> UnfollowUser(int id, int authenticatedUserId);
    }
}
