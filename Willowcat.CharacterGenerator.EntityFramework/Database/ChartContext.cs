using Microsoft.EntityFrameworkCore;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Data
{

    public class ChartContext : DbContext
    {
        public ChartContext(DbContextOptions<ChartContext> options)
            : base(options)
        {
        }

        public DbSet<ChartCollectionModel> ChartCollections { get; set; }
        public DbSet<OptionModel> ChartOptions { get; set; }
        public DbSet<ChartModel> Charts { get; set; }
        public DbSet<MigrationModel> Migrations { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChartModel>().Property(m => m.Dice)
                .HasConversion(
                    dice => dice.ToString(),
                    str => Dice.Parse(str)
                );

            modelBuilder.Entity<ChartModel>()
                .HasMany(m => m.Tags)
                .WithMany();

            modelBuilder.Entity<ChartModel>()
                .HasOne(m => m.ParentChart)
                .WithMany(m => m.SubCharts)
                .HasForeignKey(m => m.ParentKey);

            modelBuilder.Entity<ChartCollectionModel>()
                .HasMany(m => m.Charts)
                .WithOne()
                .HasForeignKey(m => m.Source);

            modelBuilder.Entity<ChartCollectionModel>()
                .HasMany(m => m.SubCollections)
                .WithOne(m => m.ParentCollection)
                .HasForeignKey(m => m.ParentCollectionId);

            modelBuilder.Entity<OptionModel>().Property(m => m.Range)
                .HasConversion(
                    range => range.ToString(),
                    str => DiceRange.Parse(str)
                );

            modelBuilder.Entity<OptionModel>()
                .HasOne(m => m.Chart)
                .WithMany(m => m.Options)
                .HasForeignKey(m => m.ChartKey);

            modelBuilder.Entity<OptionModel>()
                .HasOne(m => m.GoToChart)
                .WithMany()
                .HasForeignKey(m => m.GoToChartKey);

            base.OnModelCreating(modelBuilder);
        }
    }
}
