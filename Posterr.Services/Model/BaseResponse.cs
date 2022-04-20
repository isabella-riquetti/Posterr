using System.Threading.Tasks;

namespace Posterr.Services.Model
{
    /// <summary>
    /// Create a response body to be used as default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BaseResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }


        /// <summary>
        /// Create BaseResponse of the specified type with success status and the object in the Data
        /// </summary>
        /// <param name="data">The object of the specified type</param>
        /// <returns>Response with status true and data</returns>
        public static BaseResponse<T> CreateSuccess(T data)
        {
            return new BaseResponse<T>()
            {
                Success = true,
                Data = data
            };
        }

        /// <summary>
        /// Create BaseResponse of the specified type with an error message and status false
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        /// <returns>Response with status false and the error message</returns>
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
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    public class BaseResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Create BaseResponse with no type with success status and no message
        /// </summary>
        /// <returns>Response with status true and no message</returns>
        public static BaseResponse CreateSuccess()
        {
            return new BaseResponse()
            {
                Success = true
            };
        }

        /// <summary>
        /// Create BaseResponse with no type with an error message and status false
        /// </summary>
        /// <param name="errorMessage">The error message</param>
        /// <returns>Response swith tatus false and the error message</returns>
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
