using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class BalanceConsistencyRecordEntityTypeConfiguration : IEntityTypeConfiguration<BalanceConsistencyRecord>
    {
        public void Configure(EntityTypeBuilder<BalanceConsistencyRecord> builder)
        {
            builder.Property(x => x.Currency).HasConversion(new EnumToStringConverter<Currency>());
            builder.Property(x => x.StartBalance).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.Property(x => x.EndBalance).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.Property(x => x.TotalDebit).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.Property(x => x.TotalCredit).HasColumnType(CommonInfrastructure.DecimalFormat);
            builder.HasIndex(x => x.To).IsClustered(false);
        }
    }
}
