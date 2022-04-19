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
            /* Query:
             * SELECT [t].[Id]
	         *     ,[t].[Content]
	         *     ,CONVERT(VARCHAR(100), [t].[CreatedAt])
	         *     ,[u].[Username]
	         *     ,[p0].[Id]
	         *     ,[p0].[Content]
	         *     ,[p0].[CreatedAt]
	         *     ,[p0].[OriginalPostId]
	         *     ,[p0].[UserId]
	         *     ,[u0].[Id]
	         *     ,[u0].[CreatedAt]
	         *     ,[u0].[Name]
	         *     ,[u0].[Username]
	         *     ,CONVERT(VARCHAR(100), [p0].[CreatedAt])
             * FROM (
	         *     SELECT [p].[Id]
		     *         ,[p].[Content]
		     *         ,[p].[CreatedAt]
		     *         ,[p].[OriginalPostId]
		     *         ,[p].[UserId]
	         *     FROM [Posts] AS [p]
	         *     WHERE [p].[UserId] = @__id_0
	         *     ORDER BY [p].[CreatedAt] DESC OFFSET @__p_1 ROWS FETCH NEXT @__p_2 ROWS ONLY
	         *     ) AS [t]
             * INNER JOIN [Users] AS [u] ON [t].[UserId] = [u].[Id]
             * LEFT JOIN [Posts] AS [p0] ON [t].[OriginalPostId] = [p0].[Id]
             * LEFT JOIN [Users] AS [u0] ON [p0].[UserId] = [u0].[Id]
             * ORDER BY [t].[CreatedAt] DESC
             */
            var response = await _context.Posts
                .Include(p => p.OriginalPost)
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

        public async Task<BaseResponse<PostResponseModel>> CreatePost(CreatePostRequestModel request, int authenticatedUserId)
        {
            /* Query:
             * SET NOCOUNT ON;
             * INSERT INTO [Posts] ([Content], [CreatedAt], [OriginalPostId], [UserId])
             * VALUES (@p0, @p1, @p2, @p3);
             * SELECT [Id]
             * FROM [Posts]
             * WHERE @@ROWCOUNT = 1 AND [Id] = scope_identity();
             */
            var post = new Post()
            {
                CreatedAt = request.CreatedAt,
                Content = request.Content,
                OriginalPostId = request.OriginalPostId,
                UserId = authenticatedUserId
            };
            _context.Posts.Add(post);
            _context.SaveChanges();

            /* Query:
             * SELECT TOP(1) [p].[Id], [p].[Content], CONVERT(VARCHAR(100), [p].[CreatedAt]), [u].[Username], CASE
             *     WHEN [p0].[Id] IS NOT NULL THEN CAST(1 AS bit)
             *     ELSE CAST(0 AS bit)
             * END, [p0].[Id], [u0].[Username], [p0].[Content], CONVERT(VARCHAR(100), [p0].[CreatedAt])
             * FROM [Posts] AS [p]
             * INNER JOIN [Users] AS [u] ON [p].[UserId] = [u].[Id]
             * LEFT JOIN [Posts] AS [p0] ON [p].[OriginalPostId] = [p0].[Id]
             * LEFT JOIN [Users] AS [u0] ON [p0].[UserId] = [u0].[Id]
             * WHERE [p].[Id] = @__post_Id_0
             * ORDER BY [p].[CreatedAt] DESC
             */
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
}
