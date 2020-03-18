using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Binebase.Exchange.Gateway.Persistence.Configurations
{
    public class PromotionEntityTypeConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.CurrencyAmount).HasColumnType("decimal(18,8)");
            builder.Property(x => x.TokenAmount).HasColumnType("decimal(18,8)");
            builder.HasOne(x => x.MiningRequest).WithOne();
        }
    }
}
