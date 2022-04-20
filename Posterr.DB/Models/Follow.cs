using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posterr.DB.Models
{
    public class Follow
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        [Required]
        public bool Unfollowed { get; set; }

        public int FollowerId { get; set; }
        
        [ForeignKey("FollowerId")]
        public virtual User Follower { get; set; }

        public int FollowingId { get; set; }

        [ForeignKey("FollowingId")]
        public virtual User Following { get; set; }
    }
}
