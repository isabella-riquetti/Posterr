using Posterr.DB.Models;
using System.Linq;

namespace Posterr.Infra.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get user from Users table
        /// </summary>
        /// <param name="userId">The user Id the should be looked up</param>
        /// <returns>A queryable instance of the user</returns>
        IQueryable<User> GetUser(int userId);

        /// <summary>
        /// Check if the user exists in the database
        /// </summary>
        /// <param name="userId">The user Id the should be looked up</param>
        /// <returns>If the user exist</returns>
        bool UserExists(int userId);

        /// <summary>
        /// Check if the user exist by the username and return the id case exist
        /// </summary>
        /// <param name="username">Expected username</param>
        /// <returns>The userid in case exists</returns>
        bool UserExists(string username, out int? userId);

        /// <summary>
        /// Create the user in the Users table
        /// </summary>
        /// <param name="username">The username of the user</param>
        void CreateUser(string username);
    }
}

