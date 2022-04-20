namespace Posterr.API.Helper
{
    public static class AuthMockHelper
    {
        /// <summary>
        /// Get the Authenticated User ID from the Header since we're not authenticating in the app yet
        /// </summary>
        /// <param name="request">The HttpRequest</param>
        /// <returns>The header user id or the defailt one</returns>
        public static int GetUserFromHeader(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            if (request?.Headers?.ContainsKey("AuthenticatedUserId") == true)
            {
                if (int.TryParse(request.Headers["AuthenticatedUserId"], out int userId))
                {
                    return userId;
                }
            }

            return 1;
        }
    }
}
