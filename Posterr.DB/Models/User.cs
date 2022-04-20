using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posterr.DB.Models
{
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }
        
        public string Name { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Follow> Following { get; set; }

        public virtual ICollection<Follow> Followers { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
