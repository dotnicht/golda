using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Persistence.Configurations
{
    public class TransactionEntityTypeConfiguration //: IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            //builder.HasKey(x => x.Id);
            builder.Property(x => x.Source).HasConversion(new EnumToStringConverter<TransactionSource>());
            builder.Property(x => x.Amount).HasColumnType("decimal(18,8)");
        }
    }
}
