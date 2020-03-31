using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class MiningRequestEntityTypeConfiguration : IEntityTypeConfiguration<MiningRequest>
    {
        public void Configure(EntityTypeBuilder<MiningRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<MiningType>());
            builder.Property(x => x.Amount).HasColumnType(CommonInfrastructure.DecimalFormat);
        }
    }
}
