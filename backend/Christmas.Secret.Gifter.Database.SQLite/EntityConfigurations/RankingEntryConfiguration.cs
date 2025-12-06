using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MSSql.Infrastructure.Entities;

namespace MSSql.Infrastructure.EntityConfigurations {
    public class RankingEntryConfiguration : IEntityTypeConfiguration<RankingEntry> {
        public void Configure(EntityTypeBuilder<RankingEntry> builder) {
            builder.HasKey(o => o.Id);
            builder.Property(p => p.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
