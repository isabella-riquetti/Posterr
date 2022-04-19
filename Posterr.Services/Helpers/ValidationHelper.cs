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
        /// <param name="skip">The skip number</param>
        /// <param name="errorMessage">Error message, case any</param>
        /// <returns>The skip is valid or not</returns>
        public static bool IsSkipPagePossible(int? skip, out string errorMessage)
        {
            if (skip < 0)
            {
                errorMessage = "Cannot skip negative number of records";
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
    }
}
