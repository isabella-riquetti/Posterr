using System;
using System.Collections.Generic;

namespace Posterr.DB.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }

        public IList<Follow> Following { get; set; }
        public IList<Follow> Followers { get; set; }

        public IList<Post> Posts { get; set; }
    }
}
