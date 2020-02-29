using Binebase.Exchange.Gateway.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Persistence.Configurations
{
    public class MiningRequestEntityTypeConfiguration : IEntityTypeConfiguration<MiningRequest>
    {
        public void Configure(EntityTypeBuilder<MiningRequest> builder)
        {
            builder.HasKey(x => x.Id);
        }
    }
}
