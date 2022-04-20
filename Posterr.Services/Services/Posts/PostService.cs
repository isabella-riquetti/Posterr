using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Posterr.Services
{
    public class PostService : IPostService
    {
        private readonly ApiContext _context;

        public PostService(ApiContext context)
        {
            _context = context;
        }

        public async Task<BaseResponse<IList<PostResponseModel>>> GetUserPosts(int userId, int skipPages = 0, int pageSize = 5)
        {
            var response = await _context.Posts
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skipPages * pageSize)
                .Take(pageSize)
                .Select(p => new PostsModel
                {
                    PostId = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt.ToString(),
                    Username = p.User.Username,
                    OriginalPost = p.OriginalPost != null ? new PostsModel
                    {
                        PostId = p.OriginalPost.Id,
                        Username = p.OriginalPost.User.Username,
                        Content = p.OriginalPost.Content,
                        CreatedAt = p.OriginalPost.CreatedAt.ToString()
                    } : null
                })
                .ToListAsync();

            IList<PostResponseModel> formatedResponse = response.Select(r => new PostResponseModel(r)).ToList();

            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(formatedResponse);
        }

        public async Task<BaseResponse<IList<PostResponseModel>>> GetUserFollowingTimeline(int userId, int skipPages = 0, int pageSize = 10)
        {
            var response = await _context.Follows
                .Where(f => f.FollowerId == userId && !f.Unfollowed)
                .SelectMany(f => f.Following.Posts.Select(p => new BasicPostModel
                {
                    PostId = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    Username = p.User.Username,
                    OriginalPostId = p.OriginalPostId,
                    OriginalPostCreatedAt = p.OriginalPost != null ? p.OriginalPost.CreatedAt : null,
                    OriginalPostContent = p.OriginalPost != null ? p.OriginalPost.Content : null,
                    OriginalPostUsername = p.OriginalPost != null ? p.OriginalPost.User.Username : null
                }))
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skipPages * pageSize)
                .Take(pageSize)
                .ToListAsync();

            IList<PostsModel> postModels = response.Select(r => new PostsModel(r)).ToList();  
            IList<PostResponseModel> formatedResponse = postModels.Select(r => new PostResponseModel(r)).ToList();

            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(formatedResponse);
        }

        public async Task<BaseResponse<IList<PostResponseModel>>> GetTimeline(int skipPages = 0, int pageSize = 10)
        {
            var response = await _context.Posts
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skipPages * pageSize)
                .Take(pageSize)
                .Select(p => new PostsModel
                {
                    PostId = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt.ToString(),
                    Username = p.User.Username,
                    OriginalPost = p.OriginalPost != null ? new PostsModel
                    {
                        PostId = p.OriginalPost.Id,
                        Username = p.OriginalPost.User.Username,
                        Content = p.OriginalPost.Content,
                        CreatedAt = p.OriginalPost.CreatedAt.ToString()
                    } : null
                })
                .ToListAsync();
            
            IList<PostResponseModel> formatedResponse = response.Select(r => new PostResponseModel(r)).ToList();

            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(formatedResponse);
        }

        public async Task<BaseResponse<IList<PostResponseModel>>> SearchByText(string text, int skipPages, int pageSize = 10)
        {
            var response = await _context.Posts
                .Where(p => p.Content.Contains(text))
                .OrderByDescending(s => s.CreatedAt)
                .Skip(skipPages * pageSize)
                .Take(pageSize)
                .Select(p => new PostsModel
                {
                    PostId = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt.ToString(),
                    Username = p.User.Username,
                    OriginalPost = p.OriginalPost != null ? new PostsModel
                    {
                        PostId = p.OriginalPost.Id,
                        Username = p.OriginalPost.User.Username,
                        Content = p.OriginalPost.Content,
                        CreatedAt = p.OriginalPost.CreatedAt.ToString()
                    } : null
                })
                .ToListAsync();

            IList<PostResponseModel> formatedResponse = response.Select(r => new PostResponseModel(r)).ToList();

            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(formatedResponse);
        }

        public async Task<BaseResponse<PostResponseModel>> CreatePost(CreatePostRequestModel request, int authenticatedUserId)
        {
            var post = new Post()
            {
                CreatedAt = request.CreatedAt,
                Content = request.Content,
                OriginalPostId = request.OriginalPostId,
                UserId = authenticatedUserId
            };
            _context.Posts.Add(post);
            _context.SaveChanges();
            
            var response = await _context.Posts
                .Include(p => p.OriginalPost)
                .Where(p => p.Id == post.Id)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostsModel
                {
                    PostId = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt.ToString(),
                    Username = p.User.Username,
                    OriginalPost = p.OriginalPost != null ? new PostsModel
                    {
                        PostId = p.OriginalPost.Id,
                        Username = p.OriginalPost.User.Username,
                        Content = p.OriginalPost.Content,
                        CreatedAt = p.OriginalPost.CreatedAt.ToString()
                    } : null
                })
                .FirstOrDefaultAsync();
            return BaseResponse<PostResponseModel>.CreateSuccess(new PostResponseModel(response));
        }
    }

    public class BasicPostModel
    {
        public int PostId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; internal set; }
        public string Username { get; internal set; }
        
        public int? OriginalPostId { get; internal set; }
        public DateTime? OriginalPostCreatedAt { get; set; }
        public string OriginalPostContent { get; internal set; }
        public string OriginalPostUsername { get; internal set; }
    }
}
