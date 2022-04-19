using Posterr.Services.Model;
using System;

namespace Posterr.Services.Helpers
{
    public class ValidationHelper
    {
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

        public static bool IsValidUser(int id, Func<int, BaseResponse> userExist, out string errorMessage)
        {
            if (!IsValidUserId(id, out errorMessage))
            {
                return false;
            }
            BaseResponse responseUserExist = userExist(id);
            if (!responseUserExist.Success)
            {
                errorMessage = responseUserExist.Message;
                return false;
            }

            errorMessage = null;
            return true;
        }

        public static bool IsSkipPossible(int? skip, out string errorMessage)
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
