using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSSql.Infrastructure.Entities;
using System.Reflection;

namespace MSSql.Infrastructure {
    public sealed class DataDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public DataDbContext(DbContextOptions<DataDbContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<AlgorithmResultEntry> Results { get; set; }
        public DbSet<AlgorithmSummaryEntry> Summaries { get; set; }
        public DbSet<RankingEntry> Ranks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Seed data
            modelBuilder.Entity<RankingEntry>().HasData(
                new RankingEntry { Id = Guid.NewGuid().ToString(), AlgorithmId = Configuration.Common.Constants.AGENT_NAME_A, Score = 1 },
                new RankingEntry { Id = Guid.NewGuid().ToString(), AlgorithmId = Configuration.Common.Constants.AGENT_NAME_B, Score = 1 },
                new RankingEntry { Id = Guid.NewGuid().ToString(), AlgorithmId = Configuration.Common.Constants.AGENT_NAME_C, Score = 1 },
                new RankingEntry { Id = Guid.NewGuid().ToString(), AlgorithmId = Configuration.Common.Constants.AGENT_NAME_D, Score = 1 },
                new RankingEntry { Id = Guid.NewGuid().ToString(), AlgorithmId = Configuration.Common.Constants.AGENT_NAME_E, Score = 1 }
            );
        }
    }
}
