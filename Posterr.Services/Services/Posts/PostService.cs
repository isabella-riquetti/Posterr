using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using Posterr.Services.Model;
using Posterr.Services.User;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Posterr.Services
{
    public class PostService : IPostService, IUserTimelineService, ITimelineService
    {
        private readonly IPostRepository _postRepository;

        public PostService(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        public BaseResponse<IList<PostResponseModel>> GetUserPosts(int userId, int skipPages = 0, int pageSize = 5)
        {
            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(_CreatePostModel(_postRepository.GetPostsByUserId(userId, skipPages, pageSize)));
        }

        public BaseResponse<IList<PostResponseModel>> GetUserFollowingTimeline(int userId, int skipPages = 0, int pageSize = 10)
        {
            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(_CreateBasicPostModel(_postRepository.GetFollowedPosts(userId, skipPages, pageSize)));
        }

        public BaseResponse<IList<PostResponseModel>> GetTimeline(int skipPages = 0, int pageSize = 10)
        {
            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(_CreatePostModel(_postRepository.GetTimelinePosts(skipPages, pageSize)));
        }

        public BaseResponse<IList<PostResponseModel>> SearchByText(string text, int skipPages, int pageSize = 10)
        {
            return BaseResponse<IList<PostResponseModel>>.CreateSuccess(_CreatePostModel(_postRepository.GetPostsByPartialTextSearch(text, skipPages, pageSize)));
        }

        public BaseResponse<PostResponseModel> CreatePost(CreatePostRequestModel request, int authenticatedUserId)
        {
            var userLastPosts = _postRepository.GetPostsByUserId(authenticatedUserId)
                .Select(p => p.CreatedAt.Date);
            if (userLastPosts.Count() == 5 && userLastPosts.LastOrDefault() >= DateTime.Now.Date)
            {
                return BaseResponse<PostResponseModel>.CreateError("You can't post more than 5 times per day");
            }

            if (request.OriginalPostId != null)
            {
                Post referencedPost = _postRepository.GetPostsById((int)request.OriginalPostId).FirstOrDefault();
                if (referencedPost == null)
                {
                    return BaseResponse<PostResponseModel>.CreateError("Original Post not found");
                }
                else if (String.IsNullOrEmpty(referencedPost.Content))
                {
                    return BaseResponse<PostResponseModel>.CreateError("Can't repost a repost");
                }
            }

            var post = _postRepository.CreatePost(authenticatedUserId, request.Content, request.CreatedAt, request.OriginalPostId);

            IList<PostResponseModel> newPostFormatted = _CreatePostModel(_postRepository.GetPostsById(post.Id));

            return BaseResponse<PostResponseModel>.CreateSuccess(newPostFormatted.FirstOrDefault());
        }

        private static IList<PostResponseModel> _CreatePostModel(IQueryable<Post> posts)
        {
            var response = posts
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
                .ToList();

            return response.Select(r => new PostResponseModel(r)).ToList();
        }

        private static IList<PostResponseModel> _CreateBasicPostModel(IQueryable<Post> posts)
        {
            var response = posts
                .Select(p => new BasicPostModel
                {
                    PostId = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt,
                    Username = p.User.Username,
                    OriginalPostId = p.OriginalPostId,
                    OriginalPostCreatedAt = p.OriginalPost != null ? p.OriginalPost.CreatedAt : null,
                    OriginalPostContent = p.OriginalPost != null ? p.OriginalPost.Content : null,
                    OriginalPostUsername = p.OriginalPost != null ? p.OriginalPost.User.Username : null
                })
                .ToList();

            IList<PostsModel> postModels = response.Select(r => new PostsModel(r)).ToList();
            IList<PostResponseModel> formatedResponse = postModels.Select(r => new PostResponseModel(r)).ToList();

            return formatedResponse;
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
