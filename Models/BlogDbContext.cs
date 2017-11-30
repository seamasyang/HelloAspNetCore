using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace HelloAspNetCore.Models
{
    public class NetCoreBlogContext : DbContext
    {
        public NetCoreBlogContext(DbContextOptions<NetCoreBlogContext> options)
                : base(options)
        {
        }

        public DbSet<Category> Category { get; set; }
        public DbSet<Blog> Blog { get; set; }

        public DbSet<Attachment> Attachment { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Blogs)
                .WithOne(b => b.Category)
                .HasForeignKey(b => b.CategoryID);

            modelBuilder.Entity<Blog>().Ignore(b => b.Attachment);



            /*modelBuilder.Entity<Blog>()
               .HasOne(b=>b.Category)
               .WithMany(c=>c.Blogs)
               .HasForeignKey(b => b.CategoryID);*/
        }
    }
}
