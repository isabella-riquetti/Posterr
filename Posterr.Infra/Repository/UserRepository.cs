﻿using Microsoft.EntityFrameworkCore;
using Posterr.DB;
using Posterr.DB.Models;
using Posterr.Infra.Interfaces;
using System;
using System.Linq;

namespace Posterr.Infra.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApiContext _context;

        public UserRepository(ApiContext context)
        {
            _context = context;
        }

        public IQueryable<User> GetUser(int userId)
        {
            return _context.Users
                .Where(u => u.Id == userId)
                .AsQueryable();
        }

        public bool UserExists(int userId)
        {
            return GetUser(userId).Any();
        }

        public bool UserExists(string username, out int? userId)
        {
            int response = _context.Users
                .Where(u => u.Username == username)
                .Select(u => u.Id)
                .FirstOrDefault();

            userId = response == 0 ? (int?)null : response;
            return userId != null;
        }

        public void CreateUser(string username)
        {
            var user = new User()
            {
                Username = username,
                Name = username,
                CreatedAt = DateTime.Now
            };
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}
