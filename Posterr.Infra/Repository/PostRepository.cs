using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using System;
using System.Linq;

namespace Posterr.Infra.Repository
{
    public class PostRepository : IPostRepository
    {
        private readonly ApiContext _context;

        public PostRepository(ApiContext context)
        {
            _context = context;
        }

        public IQueryable<Post> GetPostsById(int id)
        {
            return _OrderByAndTake(_context.Posts
                .Where(p => p.Id == id), 0, 1);
        }

        public IQueryable<Post> GetPostsByUserId(int userId, int skipPages = 0, int pageSize = 5)
        {
            return _OrderByAndTake(_context.Posts
                .Where(p => p.UserId == userId), skipPages, pageSize);
        }

        public IQueryable<Post> GetFollowedPosts(int userId, int skipPages = 0, int pageSize = 10)
        {
            return _context.Follows
                .Where(f => f.FollowerId == userId && !f.Unfollowed)
                .SelectMany(f => f.Following.Posts)
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skipPages * pageSize)
                .Take(pageSize);
        }

        public IQueryable<Post> GetTimelinePosts(int skipPages = 0, int pageSize = 10)
        {
            return _OrderByAndTake(_context.Posts, skipPages, pageSize);
        }

        public IQueryable<Post> GetPostsByPartialTextSearch(string text, int skipPages, int pageSize = 10)
        {
            return _OrderByAndTake(_context.Posts
                .Where(p => p.Content.Contains(text)), skipPages, pageSize);
        }

        public Post CreatePost(int authenticatedUserId, string content, DateTime createdAt, int? originalPostId = null)
        {
            var post = new Post()
            {
                UserId = authenticatedUserId,
                Content = content,
                CreatedAt = createdAt,
                OriginalPostId = originalPostId
            };
            _context.Posts.Add(post);
            _context.SaveChanges();

            return post;
        }

        private IQueryable<Post> _OrderByAndTake(IQueryable<Post> posts, int skipPages = 0, int pageSize = 10)
        {
            return posts
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skipPages * pageSize)
                .Take(pageSize);
        }
    }
}
