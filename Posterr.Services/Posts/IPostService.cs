using Posterr.Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Posterr.Services.User
{
    public interface IPostService
    {
        public Task<IList<PostResponseModel>> GetUserPosts(int id, int skip, int limit = 5);
    }
}
