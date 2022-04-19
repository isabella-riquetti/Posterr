using System;

namespace Posterr.Services.Model
{
    /// <summary>
    /// Used to create a post
    /// This could be a query parameter, but in the future new feature would probably be added to the API and would be better to be a body
    /// </summary>
    public class CreatePostRequestModel
    {
        public CreatePostRequestModel() : this(DateTime.Now)
        {
        }
        
        public CreatePostRequestModel(DateTime dateTime)
        {
            this.CreatedAt = dateTime;
        }
        
        public string Content { get; set; }
        public int OriginalPostId { get; set; }
        internal DateTime CreatedAt { get; private set; }
    }
}
