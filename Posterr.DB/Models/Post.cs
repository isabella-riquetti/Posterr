using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posterr.DB.Models
{
    public class Post
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public string Content { get; set; }

        [Required]
        public int UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int? OriginalPostId { get; set; }
        
        [ForeignKey("OriginalPostId")]
        public virtual Post OriginalPost { get; set; }

        public virtual ICollection<Post> Reposts { get; set; }
    }
}
