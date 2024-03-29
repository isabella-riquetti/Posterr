﻿using Posterr.Services.Model;

namespace Posterr.Services.User
{
    public interface IFollowService
    {
        /// <summary>
        /// Follow the user with the given ID, by setting creating the Follow or by setting the Unfollowed property to false.
        /// </summary>
        /// <param name="userId">User that should be followed</param>
        /// <param name="authenticatedUserId">The current authenticated user that should follow the userId</param>
        /// <returns>Status for the request and error message, case any</returns>
        BaseResponse FollowUser(int userId, int authenticatedUserId);

        /// <summary>
        /// Unfollow the user with the given ID, by setting Unfollowed property to true.
        /// </summary>
        /// <param name="userId">User that should be unfollowed</param>
        /// <param name="authenticatedUserId">The current authenticated user that should unfollow the userId</param>
        /// <returns>Status for the request and error message, case any</returns>
        BaseResponse UnfollowUser(int userId, int authenticatedUserId);
    }
}
