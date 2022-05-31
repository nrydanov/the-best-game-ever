using System;
using DAL.GameObjects;
using DAL.Games;
using DAL.Players;
using DAL.Sessions;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class GameContext : DbContext
    {

        public virtual DbSet<Game> Games { get; set; } = null!;
        public virtual DbSet<Player> Players { get; set; } = null!;
        public virtual DbSet<Session> Sessions { get; set; } = null!;
        public virtual DbSet<GameObjectEnt> GameObjectEnt { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseNpgsql(
                    "User ID=postgres;Password=test_pswd;Host=127.0.0.1;Port=5432;Database=postgres;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Game>(entity =>
            {
                entity.ToTable("Games");

                entity.Property(f => f.Id).ValueGeneratedOnAdd();

                entity.HasKey("Id");

                entity.Property(e => e.HostId);

                entity.Property(e => e.Created).HasColumnType("datetime");
            });

            modelBuilder.Entity<Player>(entity =>
            {
                entity.ToTable("Players");

                entity.Property(f => f.Id).ValueGeneratedOnAdd();
                
                entity.HasKey("Id");

                entity.Property(e => e.Username)
                    .HasMaxLength(30)
                    .IsFixedLength();

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Session>(entity =>
            {
                entity.ToTable("Sessions");

                entity.Property(e => e.GameId);
                entity.Property(e => e.UserId);
                entity.Property(e => e.HeroId);

                entity.HasKey("UserId");
            });

            modelBuilder.Entity<GameObjectEnt>(entity =>
            {
                entity.ToTable("GameObjects");
                entity.Property(f => f.Id).ValueGeneratedOnAdd();
                entity.HasKey("Id");
            });
        }
    }
}
