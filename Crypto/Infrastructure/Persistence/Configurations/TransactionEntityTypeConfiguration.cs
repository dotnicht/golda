using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.CryptoService.Infrastructure.Persistence.Configurations
{
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Amount).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.Property(x => x.Direction).HasConversion(new EnumToStringConverter<TransactionDirection>());
            builder.Property(x => x.Hash).IsRequired();
            builder.HasOne(x => x.Address).WithMany(x => x.Transactions);
        }
    }
}
