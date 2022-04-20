using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using System;
using System.Collections.Generic;

namespace Posterr.Tests
{
    public class DatatbaseTestInput
    {
        public List<User> UsersToAdd { get; set; }
        public List<Post> PostsToAdd { get; set; }
        public List<Follow> FollowsToAdd { get; set; }

        /// <summary>
        /// Create InMemoryContext to be used in tests
        /// </summary>
        /// <returns>The context</returns>
        public ApiContext CreateNewInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApiContext>()
                   .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                   .Options;

            var apiContext = new ApiContext(options);
            _AddValues(apiContext);
            return apiContext;
        }

        /// <summary>
        /// Save one by one to allow create "old" relationships
        /// </summary>
        /// <param name="context">The context</param>
        /// <param name="input">The values that shold be added</param>
        private void _AddValues(ApiContext context)
        {
            if (UsersToAdd != null)
            {
                foreach (User user in this.UsersToAdd)
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
            }
            if (PostsToAdd != null)
            {
                foreach (Post post in this.PostsToAdd)
                {
                    context.Posts.Add(post);
                    context.SaveChanges();
                }
            }
            if (FollowsToAdd != null)
            {
                foreach (Follow follow in this.FollowsToAdd)
                {
                    context.Follows.Add(follow);
                    context.SaveChanges();
                }
            }
        }
    }
}
