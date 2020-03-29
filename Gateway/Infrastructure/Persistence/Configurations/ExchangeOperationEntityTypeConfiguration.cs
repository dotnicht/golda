using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class ExchangeOperationEntityTypeConfiguration : IEntityTypeConfiguration<ExchangeOperation>
    {
        public void Configure(EntityTypeBuilder<ExchangeOperation> builder)
        {
            builder.OwnsOne(x => x.Pair, x =>
            {
                x.WithOwner();
                var convert = new EnumToStringConverter<Currency>();
                x.Property(y => y.Base).HasColumnName(nameof(Pair.Base)).IsRequired(true).HasConversion(convert);
                x.Property(y => y.Quote).HasColumnName(nameof(Pair.Quote)).IsRequired(true).HasConversion(convert);
                builder.Property(x => x.Amount).HasColumnType(CommonInfrastructure.DecimalFormat);
            });
        }
    }
}
