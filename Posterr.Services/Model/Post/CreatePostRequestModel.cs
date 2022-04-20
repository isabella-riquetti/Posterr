using System;

namespace Posterr.Services.Model
{
    /// <summary>
    /// Used to create a post
    /// This could be a query parameter, but in the future new feature would probably be added to the API and would be better to be a body
    /// </summary>
    public class CreatePostRequestModel
    {
        public CreatePostRequestModel(DateTime? dateTime = null)
        {
            this.CreatedAt = dateTime ?? DateTime.Now;
        }

        public string Content { get; set; }
        public int? OriginalPostId { get; set; }
        internal DateTime CreatedAt { get; private set; }
    }
}
