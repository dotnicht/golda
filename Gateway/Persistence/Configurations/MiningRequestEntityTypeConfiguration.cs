﻿using Binebase.Exchange.Gateway.Domain.Entities;
using Binebase.Exchange.Gateway.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Binebase.Exchange.Gateway.Persistence.Configurations
{
    public class MiningRequestEntityTypeConfiguration : IEntityTypeConfiguration<MiningRequest>
    {
        public void Configure(EntityTypeBuilder<MiningRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Type).HasConversion(new EnumToStringConverter<TransactionType>());
            builder.Property(x => x.Amount).HasColumnType("decimal(18,8)");
        }
    }
}
