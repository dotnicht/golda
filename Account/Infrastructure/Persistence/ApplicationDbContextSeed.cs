using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static Task SeedAsync() 
        {
            return Task.CompletedTask;
        }
    }
}
