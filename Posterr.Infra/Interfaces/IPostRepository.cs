using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using System;
using System.Linq;

namespace Posterr.Infra.Interfaces
{
    public interface IPostRepository
    {
        IQueryable<Post> GetPostsById(int id);
        
        IQueryable<Post> GetPostsByUserId(int userId, int skipPages = 0, int pageSize = 5);

        IQueryable<Post> GetFollowedPosts(int userId, int skipPages = 0, int pageSize = 10);

        IQueryable<Post> GetTimelinePosts(int skipPages = 0, int pageSize = 10);

        IQueryable<Post> GetPostsByPartialTextSearch(string text, int skipPages, int pageSize = 10);

        Post CreatePost(int authenticatedUserId, string content, DateTime createdAt, int? originalPostId = null);
    }
}
