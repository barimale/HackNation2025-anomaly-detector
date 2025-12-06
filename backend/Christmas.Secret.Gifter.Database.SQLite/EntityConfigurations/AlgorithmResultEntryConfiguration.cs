using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.EntityConfigurations {
    public class AlgorithmResultEntryConfiguration : IEntityTypeConfiguration<AlgorithmResultEntry>
    {
        public void Configure(EntityTypeBuilder<AlgorithmResultEntry> builder)
        {
            builder.HasKey(o => o.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder
              .HasOne(p => p.Summary)
              .WithMany(pp => pp.Results)
              .HasForeignKey(ppp => ppp.SummaryId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
