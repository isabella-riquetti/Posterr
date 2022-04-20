using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posterr.DB.Models
{
    public class User
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DefaultValue("getdate()")]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Follow> Following { get; set; }

        public virtual ICollection<Follow> Followers { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
