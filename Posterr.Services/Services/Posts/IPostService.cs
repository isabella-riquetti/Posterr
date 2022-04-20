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
        BaseResponse<IList<PostResponseModel>> GetUserPosts(int userId, int skipPages = 0, int pageSize = 5);

        /// <summary>
        /// Get 'pageLimit' posts, skiping 'skipPages' from the user that the 'userId' follows
        /// <param name="userId">The userId that we want to grab the posts</param>
        /// <param name="skipPages">How many pages we should skip, default is 0, so it would load the first page</param>
        /// <param name="pageSize">How many register per page we want to should, default is 10</param>
        /// <returns>Posts from the user</returns>
        BaseResponse<IList<PostResponseModel>> GetUserFollowingTimeline(int userId, int skipPages = 0, int pageSize = 10);

        /// <summary>
        /// Get the general timeline for all users
        /// </summary>
        /// <param name="skipPages">How many pages we should skip, default is 0, so it would load the first page</param>
        /// <param name="pageSize">How many register per page we want to should, default is 10</param>
        /// <returns>Posts from all users</returns>
        BaseResponse<IList<PostResponseModel>> GetTimeline(int skipPages = 0, int pageSize = 10);

        /// <summary>
        /// Search posts by partial text
        /// </summary>
        /// <param name="text">Text to be searched</param>
        /// <param name="skipPages">How many pages we should skip, default is 0, so it would load the first page</param>
        /// <param name="pageSize">How many register per page we want to should, default is 10</param>
        /// <returns>Posts that match the text</returns>
        BaseResponse<IList<PostResponseModel>> SearchByText(string text, int skipPages, int pageSize = 10);

        /// <summary>
        /// Create a new basic post
        /// </summary>
        /// <param name="request">The new post content</param>
        /// <param name="authenticatedUserId">The authenticated user id</param>
        /// <returns>The new post so it can be appended to the page</returns>
        BaseResponse<PostResponseModel> CreatePost(CreatePostRequestModel request, int authenticatedUserId);
    }
}
