namespace Posterr.Services.Model
{
    /// <summary>
    /// Used to create a post
    /// This could be a query parameter, but in the future new feature would probably be added to the API and would be better to be a body
    /// </summary>
    public class NewPostModel
    {
        public string Content { get; set; }
    }
}
