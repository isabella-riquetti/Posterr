using Posterr.Services.Model;

namespace Posterr.Services.User
{
    public interface IPostService
    {
        /// <summary>
        /// Create a new basic post
        /// </summary>
        /// <param name="request">The new post content</param>
        /// <param name="authenticatedUserId">The authenticated user id</param>
        /// <returns>The new post so it can be appended to the page</returns>
        BaseResponse<PostResponseModel> CreatePost(CreatePostRequestModel request, int authenticatedUserId);
    }
}
