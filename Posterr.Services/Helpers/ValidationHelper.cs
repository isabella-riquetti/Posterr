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
    }
}
