using Posterr.DB.Models;
using System;
using System.Collections.Generic;

namespace Posterr.Services.Model
{
    public class UserProfileModel
    {
        public int Id { get; set; }
        public string CreatedAt { get; set; }
        public string Username { get; set; }
        public int Posts { get; set; }
        public int Followers { get; set; }
        public int Following { get; set; }
        public bool Followed { get; set; }
    }
}
