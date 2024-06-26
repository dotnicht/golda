﻿using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<TransactionType>());
            builder.Property(x => x.Currency).HasConversion(new EnumToStringConverter<Currency>());
            builder.Property(x => x.Amount).HasColumnType(CommonInfrastructure.DecimalFormat);
        }
    }
}
