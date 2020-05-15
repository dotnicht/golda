using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class ExchangeRateEntityTypeConfiguration : IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            var convert = new EnumToStringConverter<Currency>();
            builder.Property(x => x.Base).HasConversion(convert);
            builder.Property(x => x.Quote).HasConversion(convert);
            builder.Property(x => x.Rate).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.HasIndex(x => x.DateTime).IsUnique(false).IsClustered(true);
            builder.HasIndex(x => x.Base).IsUnique(false).IsClustered(false);
            builder.HasIndex(x => x.Quote).IsUnique(false).IsClustered(false);
        }
    }
}
