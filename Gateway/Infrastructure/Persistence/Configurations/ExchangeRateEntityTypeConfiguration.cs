using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class ExchangeRateEntityTypeConfiguration //: IEntityTypeConfiguration<ExchangeRate>
    {
        public void Configure(EntityTypeBuilder<ExchangeRate> builder)
        {
            builder.HasKey(x => new { x.Pair.Base, x.Pair.Quote, x.DateTime });
            builder.OwnsOne(x => x.Pair, x =>
                {
                    x.WithOwner();
                    var convert = new EnumToStringConverter<Currency>();
                    x.Property(y => y.Base).HasColumnName(nameof(Pair.Base)).IsRequired(true).HasConversion(convert);
                    x.Property(y => y.Quote).HasColumnName(nameof(Pair.Quote)).IsRequired(true).HasConversion(convert);
                });
            builder.Property(x => x.Rate).HasColumnType("decimal(18,8)");
        }
    }
}
