using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class PromotionEntityTypeConfiguration : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Currency).HasConversion(new EnumToStringConverter<Currency>());
            builder.Property(x => x.CurrencyAmount).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.Property(x => x.TokenAmount).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.HasOne(x => x.MiningRequest).WithMany();
        }
    }
}
