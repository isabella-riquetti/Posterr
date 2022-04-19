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
