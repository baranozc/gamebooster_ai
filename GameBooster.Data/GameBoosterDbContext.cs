using GameBooster.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GameBooster.Data 
{
    //Projeye Identity eklediğimizden dolayı db'in AspNetUsers gibi tabloları anlaması için IdentityDbContext yaptık.
    public class GameBoosterDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int> 
    {
        public GameBoosterDbContext(DbContextOptions<GameBoosterDbContext> options) : base(options)
        {
            
        }

        // public DbSet<AppUser> AppUsers {get; set;} IdentityDb Yaptığımız için artık bu satıra ihtiyaç yok.
        public DbSet<UserSystem> UserSystems {get; set;}
        public DbSet<Game> Games { get; set; }
        public DbSet<GPU> GPUs { get; set; }
        public DbSet<CPU> CPUs { get; set; }
        public DbSet<PredictionLog> PredictionLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Ekran kartı ve İşlemci isimleri tek bir tane olmalı
            modelBuilder.Entity<GPU>().HasIndex(g => g.Name).IsUnique();
            modelBuilder.Entity<CPU>().HasIndex(c => c.Name).IsUnique();

            base.OnModelCreating(modelBuilder);
        }
    } 

    
}