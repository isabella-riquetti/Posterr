using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posterr.DB.Models
{
    public class Follow
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
        [DefaultValue("0")]
        public bool Unfollowed { get; set; }

        public int FollowerId { get; set; }
        
        [ForeignKey("FollowerId")]
        public virtual User Follower { get; set; }

        public int FollowingId { get; set; }

        [ForeignKey("FollowingId")]
        public virtual User Following { get; set; }
    }
}
