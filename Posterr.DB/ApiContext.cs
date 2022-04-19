using Microsoft.EntityFrameworkCore;
using Posterr.DB.Models;

namespace Posterr.DB
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
          : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(x => x.Followers)
                .WithOne(x => x.Follower);
            
            modelBuilder.Entity<User>()
                .HasMany(x => x.Following)
                .WithOne(x => x.Following);
            
            modelBuilder.Entity<Post>()
                .HasMany(x => x.Reposts)
                .WithOne(x => x.OriginalPost);

            base.OnModelCreating(modelBuilder);
        }
    }
}
