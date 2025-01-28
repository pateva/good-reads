using GoodReads.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GoodReads.Data
{
    public class GoodReadsContext : IdentityDbContext<GoodReads.Models.User>
    {
        public GoodReadsContext(DbContextOptions<GoodReadsContext> options)
            : base(options)
        {
        }

        public DbSet<GoodReads.Models.Author> Authors { get; set; } = default!;
        public DbSet<GoodReads.Models.AuthorBook> AuthorBooks { get; set; } = default!;
        public DbSet<GoodReads.Models.Book> Books { get; set; } = default!;
        public DbSet<GoodReads.Models.BookGenre> BookGenres { get; set; } = default!;
        public DbSet<GoodReads.Models.BookStatus> BookStatuses { get; set; } = default!;
        public DbSet<GoodReads.Models.Genre> Genres { get; set; } = default!;
        public DbSet<GoodReads.Models.Note> Notes { get; set; } = default!;
        public DbSet<GoodReads.Models.User> Users { get; set; } = default!;
        public DbSet<GoodReads.Models.UserBook> UserBooks { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GoodReads.Models.AuthorBook>()
                .HasKey(ab => new { ab.AuthorId, ab.BookId });

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorId);

            modelBuilder.Entity<AuthorBook>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.BookId); ;

            modelBuilder.Entity<GoodReads.Models.BookGenre>()
               .HasKey(ab => new { ab.BookId, ab.GenreId });

            // Configure one-to-many between Genre and BookGenre
            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Genre)
                .WithMany(g => g.BookGenres)
                .HasForeignKey(bg => bg.GenreId);

            // Configure one-to-many between Book and BookGenre
            modelBuilder.Entity<BookGenre>()
                .HasOne(bg => bg.Book)
                .WithMany(b => b.BookGenres)
                .HasForeignKey(bg => bg.BookId);

            modelBuilder.Entity<GoodReads.Models.UserBook>()
               .HasKey(ab => new { ab.BookId, ab.UserId });

        }
    }
}
