using Posterr.Services.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Posterr.Services.User
{
    public interface IPostService
    {
        /// <summary>
        /// Get 'pageLimit' posts, skiping 'skipPages' from the 'userId' and return the posts in a UI friendly format
        /// </summary>
        /// <param name="userId">The userId that we want to grab the posts</param>
        /// <param name="skipPages">How many pages we should skip, default is 0, so it would load the first page</param>
        /// <param name="pageSize">How many register per page we want to should, default is 5</param>
        /// <returns>Posts from the user</returns>
        public Task<BaseResponse<IList<PostResponseModel>>> GetUserPosts(int userId, int skipPages = 0, int pageSize = 5);

        /// <summary>
        /// Create a new basic post
        /// </summary>
        /// <param name="request">The new post content</param>
        /// <param name="authenticatedUserId">The authenticated user id</param>
        /// <returns>The new post so it can be appended to the page</returns>
        Task<BaseResponse<PostResponseModel>> CreatePost(CreatePostRequestModel request, int authenticatedUserId);
    }
}
