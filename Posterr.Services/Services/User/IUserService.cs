using Posterr.Services.Model;
using System.Threading.Tasks;

namespace Posterr.Services.User
{
    public interface IUserService
    {
        public Task<BaseResponse<UserProfileModel>> GetUserProfile(int id, int autheticatedUserId);
        BaseResponse UserExist(int id);
    }
}
