using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCBookAesthetic.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using MVCBookAesthetic.Areas.Identity.Data;

namespace MVCBookAesthetic.Data
{
    public class MVCBookAestheticContext : IdentityDbContext<MVCBookAestheticUser>
    {
        public MVCBookAestheticContext (DbContextOptions<MVCBookAestheticContext> options)
            : base(options)
        {
        }

        public DbSet<MVCBookAesthetic.Models.Book> Book { get; set; } = default!;
        public DbSet<MVCBookAesthetic.Models.Author> Author { get; set; } = default!;
        public DbSet<MVCBookAesthetic.Models.Genre> Genre { get; set; } = default!;
        public DbSet<MVCBookAesthetic.Models.Review> Review { get; set; } = default!;
        public DbSet<MVCBookAesthetic.Models.UserBooks> UserBooks { get; set; } = default!;

        public DbSet<BookGenre> BookGenre { get; set; } = default!;
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<BookGenre>()
            .HasOne<Genre>(p => p.Genre)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.GenreId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<BookGenre>()
            .HasOne<Book>(p => p.Book)
            .WithMany(p => p.Genres)
            .HasForeignKey(p => p.BookId);
            //.HasPrincipalKey(p => p.Id);
            builder.Entity<Book>()
            .HasOne<Author>(p => p.Author)
            .WithMany(p => p.Books)
            .HasForeignKey(p => p.AuthorId);
            //.HasPrincipalKey(p => p.Id);
            base.OnModelCreating(builder);
        }
    }
   
}
