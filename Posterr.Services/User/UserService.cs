using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Posterr.Services
{
    public class UserService : IUserService
    {
        private readonly ApiContext _context;

        public UserService(ApiContext context)
        {
            _context = context;
        }
        
        public async Task<UserProfileModel> GetUserProfile(int id, int authenticatedUsedId)
        {
            var response = await _context.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.Following)
                .Where(u => u.Id == id)
                .Select(u => new UserProfileModel
                {
                    Id = u.Id,
                    CreatedAt = u.CreatedAt.ToShortDateString(),
                    Username = u.Username,
                    Followers = u.Followers.Count(),
                    Following = u.Following.Count(),
                    Posts = u.Posts.Count(),
                })
                .FirstOrDefaultAsync();

            return response;
        }
    }
}
