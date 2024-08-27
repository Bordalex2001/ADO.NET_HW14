using GamesClassLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace GamesClassLibrary.Data
{
    public class GamesContext : DbContext
    {
        public GamesContext() 
        { }
        
        public DbSet<Game> Games { get; set; }
        public DbSet<Studio> Studios { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies().UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=GamesDB;Integrated Security=SSPI;TrustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>().ToTable("Games");
            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
                entity.Property(g => g.Name).IsRequired().HasMaxLength(75);
                entity.HasOne(g => g.Studio).WithMany(s => s.Games).HasForeignKey(g => g.StudioId).IsRequired();
                entity.HasOne(g => g.Genre).WithMany(ge => ge.Games).HasForeignKey(g => g.GenreId).IsRequired();
                entity.Property(g => g.ReleaseDate).IsRequired();
                entity.Property(g => g.GameMode).IsRequired();
                entity.Property(g => g.CopiesAreSold);
            });
            modelBuilder.Entity<Game>().ToTable(t => t.HasCheckConstraint("CK_Game_GameMode", "GameMode IN ('Single-player', 'Multiplayer')"));

            modelBuilder.Entity<Studio>().ToTable("Studios");
            modelBuilder.Entity<Studio>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s =>  s.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Genre>().ToTable("Genres");
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.HasKey(ge => ge.Id);
                entity.Property(ge => ge.Name).IsRequired().HasMaxLength(50);
            });
            
            base.OnModelCreating(modelBuilder);
        }
    }
}