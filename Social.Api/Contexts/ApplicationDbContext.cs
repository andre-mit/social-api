using Microsoft.EntityFrameworkCore;
using Social.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Api.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Follow> Follows { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>().HasAlternateKey(x => x.Email);
            builder.Entity<User>().HasAlternateKey(x => x.UserName);

            builder.Entity<Follow>().HasKey(x => new { x.FollowerId, x.FollowingId});

            builder.Entity<Follow>().HasOne(x => x.Following).WithMany(u=> u.Following).HasForeignKey(fu => fu.FollowingId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Follow>().HasOne(x => x.Follower).WithMany(u=> u.Followers).HasForeignKey(fu => fu.FollowerId).OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(builder);
        }
    }
}
