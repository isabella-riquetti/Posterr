using System.Threading.Tasks;

namespace Posterr.Services.Model
{
    /// <summary>
    /// Create a response body to be used as default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static BaseResponse<T> CreateSuccess(T data)
        {
            return new BaseResponse<T>()
            {
                Success = true,
                Data = data
            };
        }

        public static BaseResponse<T> CreateError(string errorMessage)
        {
            return new BaseResponse<T>()
            {
                Success = false,
                Message = errorMessage
            };
        }

        /// <summary>
        /// This make the return async in case it's necessary
        /// </summary>
        /// <param name="result">The syncronous response</param>
        public static implicit operator Task<BaseResponse<T>>(BaseResponse<T> result)
        {
            return Task.FromResult(result);
        }
    }
    
    /// <summary>
    /// Create a response body to be used as default with no type
    /// </summary>
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static BaseResponse CreateSuccess()
        {
            return new BaseResponse()
            {
                Success = true
            };
        }

        public static BaseResponse CreateSuccess(string successMessage)
        {
            return new BaseResponse()
            {
                Success = true,
                Message = successMessage
            };
        }

        public static BaseResponse CreateError(string errorMessage)
        {
            return new BaseResponse()
            {
                Success = false,
                Message = errorMessage
            };
        }

        /// <summary>
        /// This make the return async in case it's necessary
        /// </summary>
        /// <param name="result">The syncronous response</param>
        public static implicit operator Task<BaseResponse>(BaseResponse result)
        {
            return Task.FromResult(result);
        }
    }
}
