using Posterr.Services.Model;
using System;

namespace Posterr.Services.Helpers
{
    public class ValidationHelper
    {
        /// <summary>
        /// Validate whether the user id provided is within the allowed range
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>The User ID is valid or not</returns>
        public static bool IsValidUserId(int id, out string errorMessage)
        {
            if (id <= 0)
            {
                errorMessage = "Invalid User Id, the ID should be between 1 and 2147483647";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Validate whether the user id provided is within the allowed range and if the user exists based on the provided funcion
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="userExists">Function to validate if the user exist</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>The user is valid or not</returns>
        public static bool IsValidUser(int id, Func<int, BaseResponse> userExists, out string errorMessage)
        {
            if (!IsValidUserId(id, out errorMessage))
            {
                return false;
            }
            BaseResponse responseUserExist = userExists(id);
            if (!responseUserExist.Success)
            {
                errorMessage = responseUserExist.Message;
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Validate whether the skip number is within the allowed range, it should be null or postivie
        /// <param name="value">The number that should be greater than 0</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>The skip is valid or not</returns>
        public static bool IsValuePositiveOrNeutral(int? value, out string errorMessage)
        {
            if (value < 0)
            {
                errorMessage = "The value cannot be negative";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Validate if username is alphanumeric and under 14 characters
        /// <param name="username">The username to be validated</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>If username is valid</returns>
        public static bool IsValidUsername(string username, out string errorMessage)
        {
            if (string.IsNullOrEmpty(username) || !System.Text.RegularExpressions.Regex.IsMatch(username, @"^[a-zA-Z0-9]{1,14}$"))
            {
                errorMessage = "Username should be alphanumeric and under 14 characters";
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Validate if the post content is not empty and under 777 charactes
        /// <param name="content">The content to be validates</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>The skip is valid or not</returns>
        public static bool IsValidPostRequest(CreatePostRequestModel request, out string errorMessage)
        {
            int minContent = 0;
            if (request == null)
            {
                errorMessage = "Request cannot be null";
                return false;
            }
            if (request.OriginalPostId == null && String.IsNullOrEmpty(request.Content))
            {
                errorMessage = "Post must have a content or be a repost";
                return false;
            }
            if (request.OriginalPostId != null && request.OriginalPostId <= 0)
            {
                errorMessage = "OriginalPostId must be positive";
                return false;
            }
            if (request.OriginalPostId != null && request.Content != null) //Is Quote Post
            {
                minContent = 1;
            }
            if (!IsValidContentLength(request.Content, out errorMessage, min: minContent))
            {
                return false;
            }

            errorMessage = null;
            return true;
        }

        /// <summary>
        /// Validate if the post content is not empty and under 777 charactes
        /// <param name="content">The content to be validates</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>The skip is valid or not</returns>
        public static bool IsValidContentLength(string content, out string errorMessage, int min = 1, int max = 777)
        {
            if (content == null && min > 0)
            {
                errorMessage = "Text cannot be null";
                return false;
            }
            if (content?.Length < min || content?.Length > max)
            {
                errorMessage = $"Text should have between {min} and {max} characters";
                return false;
            }

            errorMessage = null;
            return true;
        }
    }
}
