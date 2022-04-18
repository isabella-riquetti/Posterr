using Posterr.Services.Model;
using System.Threading.Tasks;

namespace Posterr.Services.User
{
    public interface IUserService
    {
        public Task<UserProfileModel> GetUserProfile(int id, int autheticatedUserId);
    }
}
