using System.Collections.Generic;

namespace Posterr.DB.Models
{
    public class Post
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Content { get; set; }
        public IList<User> Retweets { get; set; }
    }
}
