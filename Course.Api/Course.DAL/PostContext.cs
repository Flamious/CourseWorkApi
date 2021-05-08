using Course.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL
{
    public class PostContext : IdentityDbContext
    {
        #region Constructor
        public PostContext(DbContextOptions<PostContext> options) : base(options)
        { 

        }
        #endregion
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Commentary> Commentary { get; set; }
        public virtual DbSet<Assessment> Assessment { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.UserName).IsRequired();
            });
            modelBuilder.Entity<Post>(entity =>
            {
                entity.Property(e => e.Content).IsRequired();
                entity.HasOne(d => d.User)
                .WithMany(p => p.Post)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Commentary>(entity =>
            {
                entity.Property(e => e.Message).IsRequired();

                entity.HasOne(d => d.Post)
                .WithMany(p => p.Commentary)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(d => d.User)
                .WithMany(p => p.Commentary)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<Assessment>(entity =>
            {
                entity.Property(e => e.Like).IsRequired();

                entity.HasOne(d => d.Post)
                .WithMany(p => p.Assessment)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.Restrict);


                entity.HasOne(d => d.User)
                .WithMany(p => p.Assessment)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}

