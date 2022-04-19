using System;
using System.Collections.Generic;

namespace Posterr.DB.Models
{
    public class Post
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Content { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int? OriginalPostId { get; set; }
        public Post OriginalPost { get; set; }

        public IList<Post> Reposts { get; set; }
    }
}
