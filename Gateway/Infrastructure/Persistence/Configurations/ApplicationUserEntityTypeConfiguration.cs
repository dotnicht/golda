using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Binebase.Exchange.Gateway.Infrastructure.Persistence.Configurations
{
    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasOne(x => x.ReferralUser).WithMany(x => x.Refferals).HasForeignKey(x => x.ReferralId);
        }
    }
}
