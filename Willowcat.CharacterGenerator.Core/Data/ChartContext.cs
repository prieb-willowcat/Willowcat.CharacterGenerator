using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Willowcat.CharacterGenerator.Core.Models;
using Willowcat.CharacterGenerator.Model;

namespace Willowcat.CharacterGenerator.Core.Data
{

    public class ChartContext : DbContext
    {
        private static readonly IDatabaseMigration<ChartContext>[] _DatabaseMigrations = new IDatabaseMigration<ChartContext>[]
        {
            new InitialChartDatabaseMigration(),
        };

        private readonly DatabaseConfiguration _Configuration;

        public string DatabaseLocation => _Configuration.DatabaseLocation;
        public DbSet<ChartCollectionModel> ChartCollections { get; set; }
        public DbSet<OptionModel> ChartOptions { get; set; }
        public DbSet<ChartModel> Charts { get; set; }
        public DbSet<MigrationModel> Migrations { get; set; }
        internal IProgress<ChartSetupMessage> ProgressReporter { get; set; }
        public DbSet<TagModel> Tags { get; set; }

        public DatabaseConfiguration Configuration => _Configuration;

        public ChartContext(DatabaseConfiguration options)
        {
            _Configuration = options;
        }

        private async Task<bool> ApplyMigrationAsync(bool success, int migrationId, IDatabaseMigration<ChartContext> migration, CancellationToken cancellationToken)
        {
            if (!success) return success;

            if (Migrations.Find(migrationId) == null)
            {
                Report(new ChartSetupMessage(migration, "Bringing up"));
                success = await migration.BringUpAsync(this, cancellationToken);
                if (success)
                {
                    await Migrations.AddAsync(new MigrationModel()
                    {
                        Name = migration.GetType().Name,
                        MigrationId = migrationId,
                        DateRan = DateTime.Now,
                    });
                    await SaveChangesAsync();
                }
            }
            return success;
        }

        public static void ClearOldDatabase(string databaseLocation)
        {
            if (File.Exists(databaseLocation))
            {
                File.Delete(databaseLocation);
            }
        }

        public async Task<bool> InitializeAsync(IProgress<ChartSetupMessage> progressReporter, CancellationToken cancellationToken)
        {
            bool success = true;
            int index = 0;
            ProgressReporter = progressReporter;
            foreach (var migration in _DatabaseMigrations)
            {
                index++;
                success = await ApplyMigrationAsync(success, index, migration, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }
            ProgressReporter = null;
            return success;
        }

        internal void Report(ChartSetupMessage chartSetupMessage)
        {
            ProgressReporter?.Report(chartSetupMessage);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={_Configuration.DatabaseLocation}");
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
