using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.EntityConfigurations {
    public class AlgorithmSummaryEntryConfiguration : IEntityTypeConfiguration<AlgorithmSummaryEntry> {
        public void Configure(EntityTypeBuilder<AlgorithmSummaryEntry> builder) {
            builder.HasKey(o => o.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
