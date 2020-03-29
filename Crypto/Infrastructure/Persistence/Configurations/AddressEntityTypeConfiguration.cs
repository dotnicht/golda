using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Numerics;

namespace Binebase.Exchange.CryptoService.Infrastructure.Persistence.Configurations
{
    public class AddressEntityTypeConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<AddressType>());
            builder.Property(x => x.Currency).HasConversion(new EnumToStringConverter<Currency>());
            builder.Property(x => x.Public).IsRequired();
        }
    }
}
