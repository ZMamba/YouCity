using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DProject.Models;

namespace DProject.Data
{
    public class DProjectContext : DbContext
    {
        public DProjectContext(DbContextOptions<DProjectContext> options) : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Proposition> Propositions { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<City>().ToTable("City");
            modelBuilder.Entity<Proposition>().ToTable("Proposition");
            modelBuilder.Entity<Comment>().ToTable("Comment").HasOne<User>(u =>u.User).WithMany(
                c=>c.Comments).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Like>().ToTable("Like").HasOne<User>(u => u.User).WithMany(
                c => c.Likes).HasForeignKey(k => k.UserId).OnDelete(DeleteBehavior.Restrict); ;

            string adminRoleName = "admin";
            string userRoleName = "user";

            string adminEmail = "admin@mail";
            string adminPassword = "123456";

            // добавляем роли
            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { Id = 1, Email = adminEmail, Password = adminPassword, RoleId = adminRole.Id };

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}