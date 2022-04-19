using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.Services.Model;
using Posterr.Services.User;
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

        public async Task<IList<PostResponseModel>> GetUserPosts(int id, int skip = 0, int limit = 5)
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
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip * limit)
                .Take(limit)
                .Select(p => new PostModel
                {
                    Id = p.Id,
                    Content = p.Content,
                    CreatedAt = p.CreatedAt.ToString(),
                    Username = p.User.Username,
                    OriginalPost = p.OriginalPost != null ? new PostModel
                    {
                        Id = p.OriginalPost.Id,
                        Username = p.OriginalPost.User.Username,
                        Content = p.OriginalPost.Content,
                        CreatedAt = p.OriginalPost.CreatedAt.ToString()
                    } : null
                })
                .ToListAsync();

            IList<PostResponseModel> formatedResponse = response.Select(r => new PostResponseModel(r)).ToList();

            return formatedResponse;
        }
    }
}
